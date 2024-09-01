using Base.Domain.Contracts;

namespace App.DAL.Contracts;
// ReSharper disable once InconsistentNaming
public interface IAppUOW : IBaseUOW
{
    // list all the repositories
    IShoppingCartRepository ShoppingCartRepository { get; }
    IShopItemRepository ShopItemRepository { get; }
}