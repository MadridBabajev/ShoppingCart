using App.Domain.Entities;
using AutoMapper;
using Base.Mapper;
using Public.DTO.v1.ShopItems;

namespace Public.DTO.Mappers;

public class ShopItemListElemMapper : BaseMapper<ShopItem, ShopItemListElement>
{
    public ShopItemListElemMapper(IMapper mapper) : base(mapper) { }

    // This method should map a ShoppingCartItem to a ShopItemListElement
    public ShopItemListElement? MapShoppingCartItemToShopItemListElem(ShoppingCartItem shoppingCartItem)
    {
        var shopItemListElem = Mapper.Map<ShopItemListElement>(shoppingCartItem.Item);
        if (shopItemListElem != null)
        {
            shopItemListElem.QuantityTaken = shoppingCartItem.Quantity;
        }
        return shopItemListElem;
    }
}