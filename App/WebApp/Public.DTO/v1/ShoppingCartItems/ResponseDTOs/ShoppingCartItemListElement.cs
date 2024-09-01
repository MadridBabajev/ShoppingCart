using Public.DTO.v1.BaseDTO;

namespace Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

/// <summary>
/// Represents item DTO in a shopping cart list.
/// </summary>
public class ShoppingCartItemListElement: ItemDtoBase
{
    /// <summary>
    /// Quantity of the item in cart.
    /// </summary>
    public int QuantityTaken { get; set; } = default!;
}