using App.Domain.Entities;
using Base.DAL.Contract;

namespace App.DAL.Contracts;

public interface IShopItemRepository : IBaseRepository<ShopItem>
{
    // Repo custom methods can be added here
}