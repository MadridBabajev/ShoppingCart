namespace Public.DTO.v1.ShoppingCartItems.RequestDTOs;

public class ShoppingCartItemAction
{
    public Guid ItemId { get; set; }
    public ECartItemActions ItemAction { get; set; }
    public int? Quantity { get; set; }
}