using Base.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Base.DAL;

// ReSharper disable once InconsistentNaming
public abstract class EFBaseUOW<TDbContext> : IBaseUOW
    where TDbContext : DbContext
{
    protected readonly TDbContext UowDbContext;

    protected EFBaseUOW(TDbContext dataContext)
    {
        UowDbContext = dataContext;
    }
    
    public virtual async Task<int> SaveChangesAsync()
    {
        return await UowDbContext.SaveChangesAsync();
    }
}