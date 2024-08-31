using App.Domain.Identity;
using Base.Domain;
using Base.Domain.Contracts;

namespace App.Domain.Entities;

public class ShoppingCart: BaseDomainEntity, IDomainEntityId
{
    // Relationships
    public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}