using App.Domain.Entities;
using AutoMapper;
using Base.Mapper;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace App.BLL.Mappers;

public class ShopItemDetailsMapper(IMapper mapper) : BaseMapper<ShopItemDetails, ShopItem>(mapper)
{
    
};