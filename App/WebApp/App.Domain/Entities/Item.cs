using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain.Entities;

public class Item: BaseDomainEntity
{
    [MinLength(1)]
    [MaxLength(32)]
    [Required]
    public string Name { get; set; } = default!;
    [MinLength(1)]
    [MaxLength(32)]
    public string? Description { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Value must not be negative.")]
    public int Price { get; set; } = default!;
    [Range(0, int.MaxValue, ErrorMessage = "Value must not be negative.")]
    public int Amount { get; set; } = default!;
    public byte[]? ItemPicture { get; set; }
    
    // Nav
    public Guid ShoppingCartId { get; set; }
    public ShoppingCart? ShoppingCart { get; set; }
}