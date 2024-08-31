using App.Domain.Entities;
using Base.DAL.Contract;

namespace App.DAL.Contracts;

public interface IItemRepository : IBaseRepository<Item>
{
    // Repo custom methods can be added here
}