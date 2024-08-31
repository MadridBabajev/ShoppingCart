using App.Domain.Identity;
using Base.Domain;
using Base.Domain.Contracts;

namespace App.Domain.Entities;

public class ShoppingCart: BaseDomainEntity, IDomainEntityId
{
    // Nav
    public ICollection<Item>? Items { get; set; }
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}