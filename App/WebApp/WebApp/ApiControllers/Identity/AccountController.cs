using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using App.DAL;
using App.Domain.Identity;
using Asp.Versioning;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Public.DTO.v1.ApiResponses;
using Public.DTO.v1.Identity;

namespace WebApp.ApiControllers.Identity;

/// <summary>
/// Controller responsible for account related actions.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/identity/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the AccountController class.
    /// </summary>
    /// <param name="signInManager">Provides the APIs for user sign in.</param>
    /// <param name="userManager">Provides the APIs for managing user in a persistence store.</param>
    /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
    /// <param name="logger">Represents a type used to perform logging.</param>
    /// <param name="context">The database context.</param>
    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
        IConfiguration configuration, ILogger<AccountController> logger, ApplicationDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Register new user to the system.
    /// </summary>
    /// <param name="registrationData">User registration data.</param>
    /// <param name="expiresInSeconds">Optional, overrides default JWT token expiration value.</param>
    /// <returns>JWTResponse with JWT and refresh token.</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JWTResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JWTResponse>> Register([FromBody] Register registrationData,
        [FromQuery] int expiresInSeconds)
    {
        if (expiresInSeconds <= 0) expiresInSeconds = int.MaxValue;

        _logger.LogInformation("Attempting to register a new user with email {Email}", registrationData.Email);

        // Check if the user is already registered
        var existingUsers = await _userManager.Users.Where(u => u.Email == registrationData.Email).ToListAsync();

        if (existingUsers.Any())
        {
            _logger.LogWarning("User registration failed: Email {Email} is already in use", registrationData.Email);
            return FormatErrorResponse($"User with email {registrationData.Email} is already registered.");
        }

        if (!registrationData.Password.Equals(registrationData.ConfirmPassword))
        {
            _logger.LogWarning("User registration failed: Password and confirm password do not match for email {Email}", registrationData.Email);
            return FormatErrorResponse("Password and Confirmation password do not match");
        }

        // Register user
        var refreshToken = new AppRefreshToken();

        var appUser = new AppUser
        {
            Email = registrationData.Email,
            UserName = $"{registrationData.Email}",
            FirstName = registrationData.Firstname,
            LastName = registrationData.Lastname,
            AppRefreshTokens = new List<AppRefreshToken> { refreshToken }
        };
        refreshToken.AppUser = appUser;

        var result = await _userManager.CreateAsync(appUser, registrationData.Password);
        if (!result.Succeeded)
        {
            var errorDescription = result.Errors.First().Description;
            _logger.LogError("User registration failed: {ErrorDescription}", errorDescription);
            return FormatErrorResponse(errorDescription);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("User {Email} registered successfully", registrationData.Email);

        // Save user's full name into claims
        result = await _userManager.AddClaimsAsync(appUser, new List<Claim>
        {
            new(ClaimTypes.GivenName, appUser.FirstName),
            new(ClaimTypes.Surname, appUser.LastName)
        });

        if (!result.Succeeded)
        {
            _logger.LogError("Failed to add claims for user {Email}: {ErrorDescription}", appUser.Email, result.Errors.First().Description);
            return FormatErrorResponse(result.Errors.First().Description);
        }

        // Get full user from system with fixed data
        appUser = await _userManager.FindByEmailAsync(appUser.Email);
        if (appUser == null)
        {
            _logger.LogError("User {Email} not found after registration", registrationData.Email);
            return BadRequest(new RestApiErrorResponse
            {
                Status = HttpStatusCode.BadRequest,
                Error = $"User with email {registrationData.Email} is not found after registration"
            });
        }

        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);

        // Generate JWT
        (string jwt, int jwtExpiration) = GenerateJwt(claimsPrincipal.Claims, expiresInSeconds);

        var res = new JWTResponse
        {
            JWT = jwt,
            RefreshToken = refreshToken.RefreshToken,
            ExpiresIn = jwtExpiration
        };

        _logger.LogInformation("JWT generated successfully for user {Email}", appUser.Email);
        return Ok(res);
    }

    /// <summary>
    /// Log in to the system 
    /// </summary>
    /// <param name="loginData">User login info.</param>
    /// <param name="expiresInSeconds">optional, override default value.</param>
    /// <returns>JWTResponse with JWT and refresh token</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JWTResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<JWTResponse>> LogIn([FromBody] Login loginData, [FromQuery] int expiresInSeconds)
    {
        if (expiresInSeconds <= 0) expiresInSeconds = _configuration.GetValue<int>("JWT:ExpiresInSeconds");

        _logger.LogInformation("User {Email} attempting to log in", loginData.Email);

        // Verify if the user exists
        var appUser = await _userManager.Users
            .Where(u => u.Email == loginData.Email).FirstOrDefaultAsync();

        if (appUser == null)
        {
            _logger.LogWarning("Login failed: No user found with email {Email}", loginData.Email);
            return FormatErrorResponse("No user with the provided email was found");
        }

        // Verify username and password
        var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginData.Password, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Login failed: Incorrect password for user {Email}", loginData.Email);
            return FormatErrorResponse("User/Password problem");
        }

        _logger.LogInformation("User {Email} logged in successfully", loginData.Email);

        // Get claims based user
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);

        appUser.AppRefreshTokens = await _context
            .Entry(appUser)
            .Collection(a => a.AppRefreshTokens!)
            .Query()
            .Where(t => t.AppUserId == appUser.Id)
            .ToListAsync();

        // Remove expired tokens
        foreach (var userRefreshToken in appUser.AppRefreshTokens)
        {
            if (
                userRefreshToken.ExpirationDT < DateTime.UtcNow &&
                (
                    userRefreshToken.PreviousExpirationDT == null ||
                    userRefreshToken.PreviousExpirationDT < DateTime.UtcNow
                )
            )
            {
                _context.AppRefreshTokens.Remove(userRefreshToken);
            }
        }

        var refreshToken = new AppRefreshToken
        {
            AppUserId = appUser.Id
        };
        _context.AppRefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Generate JWT
        (string jwt, int jwtExpiration) = GenerateJwt(claimsPrincipal.Claims, expiresInSeconds);

        var res = new JWTResponse
        {
            JWT = jwt,
            RefreshToken = refreshToken.RefreshToken,
            ExpiresIn = jwtExpiration
        };

        _logger.LogInformation("JWT generated successfully for user {Email}", appUser.Email);
        return Ok(res);
    }

    /// <summary>
    /// Refreshes the JWT token for a user.
    /// </summary>
    /// <param name="refreshTokenModel">The refresh token model.</param>
    /// <param name="expiresInSeconds">The new JWT token expiration duration in seconds.</param>
    /// <returns>The new JWT and refresh token, or an error message if the operation fails.</returns>
    [HttpPost]
    public async Task<ActionResult> RefreshToken(
        [FromBody] RefreshTokenModel refreshTokenModel,
        [FromQuery] int expiresInSeconds)
    {
        if (expiresInSeconds <= 0) expiresInSeconds = int.MaxValue;

        _logger.LogInformation("User attempting to refresh token");

        JwtSecurityToken jwtToken;
        // Get user info from jwt
        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(refreshTokenModel.Jwt);
            if (jwtToken == null) return FormatErrorResponse("Token is missing");
        }
        catch (Exception e)
        {
            _logger.LogError("Token parsing failed: {ErrorMessage}", e.Message);
            return FormatErrorResponse($"Cant parse the token, {e.Message}");
        }

        if (!IdentityHelpers.ValidateToken(refreshTokenModel.Jwt, _configuration["JWT:Key"]!,
                _configuration["JWT:Issuer"]!,
                _configuration["JWT:Audience"]!))
        {
            _logger.LogWarning("JWT validation failed");
            return FormatErrorResponse("JWT validation fail");
        }

        var userEmail = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (userEmail == null) return FormatErrorResponse("No email in jwt");

        // Get user and tokens
        var appUser = await _userManager.Users
            .Include(u => u.AppRefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == userEmail);
        if (appUser == null)
        {
            _logger.LogWarning("Refresh token failed: User with email {Email} not found", userEmail);
            return FormatErrorResponse($"User with email {userEmail} not found");
        }

        // Load and compare refresh tokens
        await _context.Entry(appUser)
            .Collection(u => u.AppRefreshTokens!)
            .Query()
            .Where(x =>
                (x.RefreshToken == refreshTokenModel.RefreshToken && x.ExpirationDT > DateTime.UtcNow) ||
                (x.PreviousRefreshToken == refreshTokenModel.RefreshToken &&
                 x.PreviousExpirationDT > DateTime.UtcNow)
            )
            .ToListAsync();

        if (appUser.AppRefreshTokens == null || appUser.AppRefreshTokens.Count == 0)
        {
            _logger.LogWarning("Refresh token failed: No valid refresh tokens found for user {Email}", userEmail);
            return FormatErrorResponse(
                $"RefreshTokens collection is null or empty - {appUser.AppRefreshTokens?.Count}");
        }

        // Generate new jwt
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
        (string jwt, int jwtExpiration) = GenerateJwt(claimsPrincipal.Claims, expiresInSeconds);

        // Create a new refresh token, keep old one still valid for some time
        var refreshToken = appUser.AppRefreshTokens.First();
        if (refreshToken.RefreshToken == refreshTokenModel.RefreshToken)
        {
            refreshToken.PreviousRefreshToken = refreshToken.RefreshToken;
            refreshToken.PreviousExpirationDT = DateTime.UtcNow.AddMinutes(1);

            refreshToken.RefreshToken = Guid.NewGuid().ToString();
            refreshToken.ExpirationDT = DateTime.UtcNow.AddDays(14);

            await _context.SaveChangesAsync();
        }

        var res = new JWTResponse
        {
            JWT = jwt,
            RefreshToken = refreshToken.RefreshToken,
            ExpiresIn = jwtExpiration
        };

        _logger.LogInformation("JWT refreshed successfully for user {Email}", userEmail);
        return Ok(res);
    }

    /// <summary>
    /// Logs out a user by invalidating their refresh token.
    /// </summary>
    /// <param name="logout">The logout request containing the refresh token to invalidate.</param>
    /// <returns>An OK result if the logout is successful, otherwise an error message.</returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<ActionResult> Logout(
        [FromBody] Logout logout)
    {
        // Delete the refresh token - so user is kicked out after jwt expiration
        var userId = User.GetUserId();

        _logger.LogInformation("User {UserId} attempting to log out", userId);

        var appUser = await _context.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            _logger.LogWarning("Logout failed: No user found with ID {UserId}", userId);
            return FormatErrorResponse("No user was found");
        }

        await _context.Entry(appUser)
            .Collection(u => u.AppRefreshTokens!)
            .Query()
            .Where(x =>
                x.RefreshToken == logout.RefreshToken ||
                x.PreviousRefreshToken == logout.RefreshToken
            )
            .ToListAsync();

        foreach (var appRefreshToken in appUser.AppRefreshTokens!)
        {
            _context.AppRefreshTokens.Remove(appRefreshToken);
        }

        var deleteCount = await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} logged out successfully, {DeleteCount} tokens deleted", userId, deleteCount);
        return Ok(new { TokenDeleteCount = deleteCount });
    }
    
    private (string jwt, int jwtExpiration) GenerateJwt(IEnumerable<Claim> claims, int expiresInSeconds)
    {
        int expiresIn = expiresInSeconds < _configuration.GetValue<int>("JWT:ExpiresInSeconds")
            ? expiresInSeconds
            : _configuration.GetValue<int>("JWT:ExpiresInSeconds");

        var jwt = IdentityHelpers.GenerateJwt(
            claims,
            _configuration["JWT:Key"]!,
            _configuration["JWT:Issuer"]!,
            _configuration["JWT:Audience"]!,
            expiresInSeconds
        );
        return (jwt, expiresIn);
    }

    private ActionResult FormatErrorResponse(string message)
    {
        return BadRequest(new RestApiErrorResponse
        {
            Status = HttpStatusCode.BadRequest,
            Error = message
        });
    }
}
