using App.Domain.Entities;
using Base.DAL.Contract;

namespace App.DAL.Contracts;

public interface IShoppingCartRepository : IBaseRepository<ShoppingCart>
{
    // Repo custom methods can be added here
    Task AddCartItem(Guid cartId, Guid itemId, int quantity);
    Task RemoveCartItem(Guid cartId, Guid itemId);
    Task RemoveAllCartItems(Guid cartId);
    Task<IEnumerable<ShoppingCartItem>> GetCartItems(Guid cartId);
}