namespace Base.Domain.Contracts;

// ReSharper disable once InconsistentNaming
public interface IBaseUOW
{
    // Contain and create repositories
    Task<int> SaveChangesAsync();
}