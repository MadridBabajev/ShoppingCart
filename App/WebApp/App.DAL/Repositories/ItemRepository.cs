using App.DAL.Contracts;
using App.Domain.Entities;
using Base.DAL;

namespace App.DAL.Repositories;

public class ItemRepository
    (ApplicationDbContext dataContext) : EFBaseRepository<Item, ApplicationDbContext>(dataContext), IItemRepository;