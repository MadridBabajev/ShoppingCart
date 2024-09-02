using System.ComponentModel;
using App.BLL.Contracts;
using App.BLL.Mappers;
using App.DAL.Contracts;
using App.Domain.Entities;
using Base.BLL;
using Public.DTO.v1.ShoppingCartItems.RequestDTOs;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace App.BLL.Services;

public class ShopItemService(IAppUOW uow, ShopItemDetailsMapper itemDetailsMapper,
        ShopItemListElemMapper itemListElementMapper)
    : BaseEntityService<ShopItemDetails, ShopItem, IShopItemRepository>(uow.ShopItemRepository, itemDetailsMapper),
        IShopItemService
{
    public new async Task<IEnumerable<ShopItemListElement?>> AllAsync()
    {
        var items = await uow.ShopItemRepository.AllAsync();
        return items.Select(e => itemListElementMapper.Map(e)).ToList();
    }

    public new async Task<ShopItemDetails?> FindAsync(Guid id)
        => itemDetailsMapper.Map(await uow.ShopItemRepository.FindAsync(id));
    
    
    public async Task<IEnumerable<ShopItemListElement?>> GetCartItems(Guid userId)
    {
        var cartItems = await uow.ShopItemRepository.GetCartItems(userId);
        return cartItems.Select(e => itemListElementMapper.MapToShopItemListElem(e.Item!, userId)).ToList();
    }

    public async Task<ShopItemDetails?> GetCartItem(Guid userId, Guid itemId)
    {
        var cartItem = await uow.ShopItemRepository.GetCartItem(userId, itemId);
        return itemDetailsMapper.Map(cartItem);
    }

    public async Task AddRemoveCartItem(Guid userId, Guid itemId, ECartItemActions action, int? quantity)
    {
        switch (action)
        {
            case ECartItemActions.Increment:
                await uow.ShopItemRepository
                    .AddCartItem(userId, itemId);
                break;
            case ECartItemActions.Decrement:
                await uow.ShopItemRepository
                    .RemoveCartItem(userId, itemId);
                break;
            case ECartItemActions.SetAmount:
                await uow.ShopItemRepository
                    .SetCartItemQuantity(userId, itemId, quantity);
                break;
            default:
                throw new InvalidEnumArgumentException("An invalid action has been chosen");
        }
    }

    public async Task RemoveAllCartItems(Guid userId) 
        => await uow.ShopItemRepository.RemoveAllCartItems(userId);
}