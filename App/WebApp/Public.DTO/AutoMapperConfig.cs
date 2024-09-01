using App.Domain.Entities;
using AutoMapper;
using Public.DTO.v1.ShopItems;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace Public.DTO;

public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        // ShopItem to ShopItemListElement
        CreateMap<ShopItem, ShopItemListElement>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.SubjectPicture, opt => opt.MapFrom(src => src.ItemPicture))
            .ForMember(dest => dest.StockAmount, opt => opt.MapFrom(src => src.StockAmount))
            .ForMember(dest => dest.IsInCart, opt => opt.MapFrom(src => src.ShoppingCartItems != null && src.ShoppingCartItems.Any()));

        // ShopItem to ShopItemDetails
        CreateMap<ShopItem, ShopItemDetails>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.SubjectPicture, opt => opt.MapFrom(src => src.ItemPicture))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.StockAmount, opt => opt.MapFrom(src => src.StockAmount))
            .ForMember(dest => dest.IsInCart, opt => opt.MapFrom(src => src.ShoppingCartItems != null && src.ShoppingCartItems.Any()));

        // ShoppingCartItem to ShoppingCartItemListElement
        CreateMap<ShoppingCartItem, ShoppingCartItemListElement>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId))  // Assuming ItemId should map to Id
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Item!.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Item!.Price))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Item!.Rating))
            .ForMember(dest => dest.SubjectPicture, opt => opt.MapFrom(src => src.Item!.ItemPicture))
            .ForMember(dest => dest.QuantityTaken, opt => opt.MapFrom(src => src.Quantity));

        // ShoppingCartItem to ShoppingCartItemDetails
        CreateMap<ShoppingCartItem, ShoppingCartItemDetails>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId))  // Assuming ItemId should map to Id
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Item!.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Item!.Price))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Item!.Rating))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item!.Description))
            .ForMember(dest => dest.StockAmount, opt => opt.MapFrom(src => src.Item!.StockAmount))
            .ForMember(dest => dest.SubjectPicture, opt => opt.MapFrom(src => src.Item!.ItemPicture))
            .ForMember(dest => dest.QuantityTaken, opt => opt.MapFrom(src => src.Quantity));
    }
}