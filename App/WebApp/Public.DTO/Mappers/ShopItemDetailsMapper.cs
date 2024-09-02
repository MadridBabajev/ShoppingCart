using App.Domain.Entities;
using AutoMapper;
using Base.Mapper;
using Public.DTO.v1.ShopItems;

namespace Public.DTO.Mappers;

public class ShopItemDetailsMapper(IMapper mapper) : BaseMapper<ShopItem, ShopItemDetails>(mapper)
{
    
};