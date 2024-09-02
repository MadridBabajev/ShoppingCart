using System.Net;
using System.Net.Mime;
using App.BLL.Contracts;
using Asp.Versioning;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.DTO.v1.ApiResponses;
using Public.DTO.v1.ShoppingCartItems.RequestDTOs;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace WebApp.ApiControllers;

/// <summary>
/// API controller for handling requests related to shop and cart items.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ShopItemController : ControllerBase
{
    private readonly IAppBLL _bll;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopItemController"/> class.
    /// </summary>
    /// <param name="bll">The business logic layer instance.</param>
    public ShopItemController(IAppBLL bll)
    {
        _bll = bll;
    }

    /// <summary>
    /// Get a list of all items in the shopping cart.
    /// </summary>
    /// <returns>A list of items.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<ShopItemListElement>), StatusCodes.Status200OK)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<IEnumerable<ShopItemListElement?>>> GetAllCartItems()
    {
        // Get cart items from the database
        Guid userId = User.GetUserId();
        try
        {
            return Ok(await _bll.ShopItemService.GetCartItems(userId));
        }
        catch (Exception e)
        {
            return FormatErrorResponse($"Error retrieving the cart items list: {e.Message}");
        }
    }

    /// <summary>
    /// Get the cart item details.
    /// </summary>
    /// <param name="itemId">The ID of the item.</param>
    /// <returns>The details of the cart item.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ShopItemDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetShoppingCartItemDetails([FromBody] Guid itemId)
    {
        Guid userId = User.GetUserId();

        try
        {
            var item = await _bll.ShopItemService.GetCartItem(userId, itemId);
            if (item != null) return Ok(item);
            return FormatErrorResponse("Error finding the cart item:");
        }
        catch (Exception e)
        {
            return FormatErrorResponse($"Error getting the item details: {e.Message}");
        }
    }

    /// <summary>
    /// Endpoint for adding or removing a items from the shopping cart
    /// </summary>
    /// <param name="shoppingCartItemAction">The action of the user</param>
    /// <returns></returns>
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddRemoveCartItem([FromBody] ShoppingCartItemAction shoppingCartItemAction)
    {
        Guid userId = User.GetUserId();

        try
        {
            await _bll.ShopItemService
                .AddRemoveCartItem(userId, shoppingCartItemAction.ItemId, shoppingCartItemAction.ItemAction,
                    shoppingCartItemAction.Quantity);
            return Ok();
        }
        catch (Exception e)
        {
            return FormatErrorResponse($"Error adding/removing the cart item: {e.Message}");
        }
    }
    
    /// <summary>
    /// Endpoint for adding or removing a items from the shopping cart
    /// </summary>
    /// <returns></returns>
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ClearShoppingCart()
    {
        Guid userId = User.GetUserId();

        try
        {
            await _bll.ShopItemService.RemoveAllCartItems(userId);
            return Ok();
        }
        catch (Exception e)
        {
            return FormatErrorResponse($"Error occured when removing a tag: {e}");
        }
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
            var shopItems = await _bll.ShopItemService.AllAsync();
            return Ok(shopItems);
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
            var item = await _bll.ShopItemService.FindAsync(itemId);
            if (item != null) return Ok(item);
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