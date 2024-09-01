using App.Domain.Entities;
using AutoMapper;
using Base.Mapper;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace Public.DTO.Mappers;


public class ShoppingCartItemDetailsMapper : BaseMapper<ShoppingCartItem, ShoppingCartItemDetails>
{
    public ShoppingCartItemDetailsMapper(IMapper mapper) : base(mapper) { }
    
    public ShoppingCartItemDetails MapCartItemDetails(ShopItem bllSubjectDetails)
        => Mapper.Map<ShoppingCartItemDetails>(bllSubjectDetails);
    
}