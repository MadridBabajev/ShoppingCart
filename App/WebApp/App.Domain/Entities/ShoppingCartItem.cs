using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain.Entities;

public class ShoppingCartItem : BaseDomainEntity
{
    [Range(0, int.MaxValue, ErrorMessage = "Value must not be negative.")]
    public int Quantity { get; set; } = 1;

    // Relationships
    public Guid ItemId { get; set; }
    public Item? Item { get; set; }

    public Guid ShoppingCartId { get; set; }
    public ShoppingCart? ShoppingCart { get; set; }
}