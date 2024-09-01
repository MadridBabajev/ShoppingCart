using Public.DTO.v1.BaseDTO;

namespace Public.DTO.v1.ShopItems;

/// <summary>
/// Represents the details DTO an the item.
/// </summary>
public class ShopItemDetails: ItemDetailsDtoBase
{
    /// <summary>
    /// Defines whether the item has been added to the cart or not.
    /// </summary>
    public bool IsInCart = default!;
}