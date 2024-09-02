namespace Public.DTO.v1.BaseDTO;

public class ItemDetailsDtoBase: ItemDtoBase
{
    /// <summary>
    /// Description of the item.
    /// </summary>
    public string? Description { get; set; } = default!;
    /// <summary>
    /// Quantity of the item in stock.
    /// </summary>
    public int StockAmount { get; set; } = default!;
}