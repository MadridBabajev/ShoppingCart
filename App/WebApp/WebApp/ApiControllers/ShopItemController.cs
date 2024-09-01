using System.Net;
using System.Net.Mime;
using App.DAL.Contracts;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.DTO.Mappers;
using Public.DTO.v1.ApiResponses;
using Public.DTO.v1.ShopItems;

namespace WebApp.ApiControllers;

/// <summary>
/// API controller for handling requests related to shop catalog items.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ShopItemController : ControllerBase
{
    private readonly IAppUOW _uow;
    private readonly ShopItemListElemMapper _listMapper;
    private readonly ShopItemDetailsMapper _detailsMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopItemController"/> class.
    /// </summary>
    /// <param name="uow">The data access layer instance.</param>
    /// <param name="autoMapper">The AutoMapper instance.</param>
    public ShopItemController(IAppUOW uow, IMapper autoMapper)
    {
        _uow = uow;
        _listMapper = new ShopItemListElemMapper(autoMapper);
        _detailsMapper = new ShopItemDetailsMapper(autoMapper);
    }

    /// <summary>
    /// Get a list of all items in the shop.
    /// </summary>
    /// <returns>A list of all shop items.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<ShopItemListElement>), StatusCodes.Status200OK)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Optional")]
    public async Task<ActionResult<IEnumerable<ShopItemListElement?>>> GetAllShopItems()
    {
        // Get shop items from the database
        try
        {
            var items = await _uow.ShopItemRepository.AllAsync();
            return items.Select(e => _listMapper.Map(e)).ToList();
        }
        catch (Exception e)
        {
            return FormatErrorResponse($"Error retrieving the shop items list: {e.Message}");
        }
    }

    /// <summary>
    /// Get the shop item details.
    /// </summary>
    /// <param name="itemId">The ID of the item.</param>
    /// <returns>The details of the shop item.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ShopItemDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Optional")]
    public async Task<IActionResult> GetShopItemDetails([FromBody] Guid itemId)
    {
        
        try
        {
            var item = await _uow.ShopItemRepository.FindAsync(itemId);
            if (item != null) return Ok(_detailsMapper.Map(item));
            return FormatErrorResponse("Error finding the shop item:");
        }
        catch (Exception e)
        {
            return FormatErrorResponse($"Error getting the item details: {e.Message}");
        }
    }
    
    private ActionResult FormatErrorResponse(string message) {
        return BadRequest(new RestApiErrorResponse {
            Status = HttpStatusCode.BadRequest,
            Error = message
        });
    }
}