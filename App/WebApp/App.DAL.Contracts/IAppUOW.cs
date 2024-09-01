using Base.Domain.Contracts;

namespace App.DAL.Contracts;

public interface IAppUOW : IBaseUOW
{
    // list all the repositories
    IShoppingCartRepository ShoppingCartRepository { get; }
    IItemRepository ItemRepository { get; }
}