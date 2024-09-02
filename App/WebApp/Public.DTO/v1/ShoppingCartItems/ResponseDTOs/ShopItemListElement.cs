using Public.DTO.v1.BaseDTO;

namespace Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

/// <summary>
/// Represents the details DTO an the item.
/// </summary>
public class ShopItemListElement: ItemDtoBase
{
    /// <summary>
    /// Quantity of the item in stock.
    /// </summary>
    public int StockAmount { get; set; } = default!;
    /// <summary>
    /// Quantity of the item in cart.
    /// </summary>
    public int QuantityTaken { get; set; } = default!;
}