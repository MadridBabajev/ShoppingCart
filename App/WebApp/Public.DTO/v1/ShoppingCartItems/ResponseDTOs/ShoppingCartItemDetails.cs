using Public.DTO.v1.BaseDTO;

namespace Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

/// <summary>
/// Represents the details DTO an the item.
/// </summary>
public class ShoppingCartItemDetails: ItemDetailsDtoBase
{
    /// <summary>
    /// Quantity of the item in cart.
    /// </summary>
    public int QuantityTaken { get; set; } = default!;
}