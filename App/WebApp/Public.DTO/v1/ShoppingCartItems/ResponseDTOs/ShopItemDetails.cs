using Public.DTO.v1.BaseDTO;

namespace Public.DTO.v1.ShopItems;

/// <summary>
/// Represents the details DTO an the item.
/// </summary>
public class ShopItemDetails: ItemDetailsDtoBase
{
    /// <summary>
    /// Quantity of the item in cart.
    /// </summary>
    public int QuantityTaken { get; set; } = default!;
}