using App.Domain.Entities;
using AutoMapper;
using Base.Mapper;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace Public.DTO.Mappers;


public class ShoppingCartItemListElemMapper : BaseMapper<ShoppingCartItem, ShoppingCartItemListElement>
{
    public ShoppingCartItemListElemMapper(IMapper mapper) : base(mapper) { }

    public override ShoppingCartItemListElement? Map(ShoppingCartItem? entity)
    {
        if (entity == null) return null;
        
        var dto = Mapper.Map<ShoppingCartItemListElement>(entity.Item);
        if (dto != null)
        {
            dto.QuantityTaken = entity.Quantity;
        }
        return dto;
    }
}