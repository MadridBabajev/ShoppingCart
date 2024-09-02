using App.Domain.Entities;
using AutoMapper;
using Base.Mapper;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace App.BLL.Mappers;

public class ShopItemListElemMapper : BaseMapper<ShopItemListElement, ShopItem>
{
    public ShopItemListElemMapper(IMapper mapper) : base(mapper) { }
    
    public ShopItemListElement? MapToShopItemListElem(ShopItem shopItem, Guid userId)
    {
        var shopItemListElem = Mapper.Map<ShopItemListElement>(shopItem);
        if (shopItemListElem != null && shopItem.ShoppingCartItems != null)
        {
            var shoppingCartItem = shopItem.ShoppingCartItems.FirstOrDefault(e => e.AppUserId == userId);
            if (shoppingCartItem != null)
            {
                shopItemListElem.QuantityTaken = shoppingCartItem.Quantity;
            }
        }
        return shopItemListElem;
    }
}