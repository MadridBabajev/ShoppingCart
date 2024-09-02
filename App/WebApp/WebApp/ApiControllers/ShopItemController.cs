using System.Net;
using System.Net.Mime;
using App.DAL.Contracts;
using Asp.Versioning;
using AutoMapper;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.DTO.Mappers;
using Public.DTO.v1.ApiResponses;
using Public.DTO.v1.ShopItems;
using Public.DTO.v1.ShoppingCartItems.RequestDTOs;

namespace WebApp.ApiControllers;

/// <summary>
/// API controller for handling requests related to shop and cart items.
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
            var cartItems = await _uow.ShopItemRepository.GetCartItems(userId);
            return cartItems.Select(e => _listMapper.MapShoppingCartItemToShopItemListElem(e)).ToList();

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
            var item = await _uow.ShopItemRepository.GetCartItem(userId, itemId);
            if (item != null) return Ok(_detailsMapper.Map(item));
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
            switch (shoppingCartItemAction.ItemAction)
            {
                case ECartItemActions.Increment:
                    await _uow.ShopItemRepository
                        .AddCartItem(userId, shoppingCartItemAction.ItemId);
                    break;
                case ECartItemActions.Decrement:
                    await _uow.ShopItemRepository
                        .RemoveCartItem(userId, shoppingCartItemAction.ItemId);
                    break;
                case ECartItemActions.SetAmount:
                    await _uow.ShopItemRepository
                        .SetCartItemQuantity(userId, shoppingCartItemAction.ItemId, shoppingCartItemAction.Quantity);
                    break;
                default:
                    return FormatErrorResponse("Error adding/removing the cart item: Invalid cart action");
            }
            
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
            await _uow.ShopItemRepository.RemoveAllCartItems(userId);
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