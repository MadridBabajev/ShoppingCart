using App.DAL.Contracts;
using App.DAL.Repositories;
using Base.DAL;

namespace App.DAL;

// ReSharper disable InconsistentNaming
public class AppUOW(ApplicationDbContext dataContext) : EFBaseUOW<ApplicationDbContext>(dataContext), IAppUOW
{
    private IItemRepository? _itemsRepository;
    private IShoppingCartRepository? _shoppingCartRepository;

    public IItemRepository ItemRepository =>
        _itemsRepository ??= new ItemRepository(UowDbContext);
    
    public IShoppingCartRepository ShoppingCartRepository =>
        _shoppingCartRepository ??= new ShoppingCartRepository(UowDbContext);
}