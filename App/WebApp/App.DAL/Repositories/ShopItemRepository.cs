using App.DAL.Contracts;
using App.Domain.Entities;
using Base.DAL;

namespace App.DAL.Repositories;

public class ShopItemRepository
    (ApplicationDbContext dataContext) : EFBaseRepository<ShopItem, ApplicationDbContext>(dataContext), IShopItemRepository;