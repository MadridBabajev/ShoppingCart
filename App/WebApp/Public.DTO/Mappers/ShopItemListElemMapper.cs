using App.Domain.Entities;
using AutoMapper;
using Base.Mapper;
using Public.DTO.v1.ShopItems;

namespace Public.DTO.Mappers;

public class ShopItemListElemMapper(IMapper mapper) : BaseMapper<ShopItem, ShopItemListElement>(mapper);