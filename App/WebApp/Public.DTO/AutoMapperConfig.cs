using App.Domain.Entities;
using AutoMapper;
using Public.DTO.v1.ShopItems;

namespace Public.DTO;

public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<ShopItem, ShopItemDetails>().ReverseMap();
        CreateMap<ShopItem, ShopItemListElement>().ReverseMap();
    }
}