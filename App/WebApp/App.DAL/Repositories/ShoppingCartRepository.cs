using App.DAL.Contracts;
using App.Domain.Entities;
using Base.DAL;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Repositories;

public class ShoppingCartRepository
    (ApplicationDbContext dataContext) : EFBaseRepository<ShoppingCart, ApplicationDbContext>(dataContext),
        IShoppingCartRepository
{
    // Adds an item to the cart, or increments its quantity if it already exists
    public async Task AddCartItem(Guid cartId, Guid itemId, int quantity = 1)
    {
        var cart = await RepositoryDbSet
            .Include(c => c.ShoppingCartItems)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        if (cart == null)
            throw new KeyNotFoundException("Cart not found.");

        var cartItem = cart.ShoppingCartItems?.FirstOrDefault(i => i.ItemId == itemId);

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
        }
        else
        {
            cart.ShoppingCartItems ??= new List<ShoppingCartItem>();
            cart.ShoppingCartItems.Add(new ShoppingCartItem
            {
                ItemId = itemId,
                ShoppingCartId = cartId,
                Quantity = quantity
            });
        }

        await RepositoryDbContext.SaveChangesAsync();
    }

    // Decreases quantity of an item in the cart or removes it if the quantity becomes zero
    public async Task RemoveCartItem(Guid cartId, Guid itemId)
    {
        var cart = await RepositoryDbSet
            .Include(c => c.ShoppingCartItems)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        if (cart == null)
            throw new KeyNotFoundException("Cart not found.");

        var cartItem = cart.ShoppingCartItems?.FirstOrDefault(i => i.ItemId == itemId);

        if (cartItem != null)
        {
            if (cartItem.Quantity > 1) cartItem.Quantity--;
            else cart.ShoppingCartItems!.Remove(cartItem);

            await RepositoryDbContext.SaveChangesAsync();
        }
    }

    // Removes all items from the cart
    public async Task RemoveAllCartItems(Guid cartId)
    {
        var cart = await RepositoryDbSet
            .Include(c => c.ShoppingCartItems)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        if (cart == null)
            throw new KeyNotFoundException("Cart not found.");

        cart.ShoppingCartItems?.Clear();

        await RepositoryDbContext.SaveChangesAsync();
    }

    // Retrieves all items in the cart
    public async Task<IEnumerable<ShoppingCartItem>> GetCartItems(Guid cartId)
    {
        var cart = await RepositoryDbSet
            .Include(c => c.ShoppingCartItems)
            .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        return cart?.ShoppingCartItems ?? new List<ShoppingCartItem>();
    }
}
