using Base.DAL.Contract;
using Public.DTO.v1.ShoppingCartItems.RequestDTOs;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace App.BLL.Contracts;


public interface IShopItemService: IBaseRepository<ShopItemDetails>
{
    // custom methods
    Task<IEnumerable<ShopItemListElement?>> AllAsync(Guid? userId);
    Task AddRemoveCartItem(Guid userId, Guid itemId, ECartItemActions action, int? quantity);
    Task RemoveAllCartItems(Guid userId);
    Task<IEnumerable<ShopItemListElement?>> GetCartItems(Guid userId);
    public Task<ShopItemDetails?> GetCartItem(Guid userId, Guid itemId);
}