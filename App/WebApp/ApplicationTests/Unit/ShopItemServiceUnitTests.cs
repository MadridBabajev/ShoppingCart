using App.BLL.Mappers;
using App.BLL.Services;
using App.DAL.Contracts;
using App.DAL.Seeding;
using App.Domain.Entities;
using Moq;
using Public.DTO.v1.ShoppingCartItems.RequestDTOs;
using Public.DTO.v1.ShoppingCartItems.ResponseDTOs;

namespace TestProject1.Unit;

public class ShopItemServiceUnitTests
{
    private readonly Mock<IAppUOW> _uowMock;
    private readonly Mock<ShopItemDetailsMapper> _detailsMapperMock;
    private readonly Mock<ShopItemListElemMapper> _listMapperMock;
    private readonly ShopItemService _shopItemService;
    private readonly DataGuids _dataGuids;

    public ShopItemServiceUnitTests()
    {
        _uowMock = new Mock<IAppUOW>();
        _detailsMapperMock = new Mock<ShopItemDetailsMapper>();
        _listMapperMock = new Mock<ShopItemListElemMapper>();
        _shopItemService = new ShopItemService(_uowMock.Object, _detailsMapperMock.Object, _listMapperMock.Object);
        _dataGuids = new DataGuids();
    }

    [Fact]
    public async Task GetCartItem_ReturnsShopItemDetails_WhenItemExistsInCart()
    {
        // Arrange
        var itemId = _dataGuids.Item1Id;
        var userId = Guid.NewGuid();

        var shopItemDetails = new ShopItemDetails
        {
            Id = itemId,
            Name = "Sneakers",
            Rating = 4.5,
            StockAmount = 10,
            Price = 50,
            QuantityTaken = 2
        };
        
        _uowMock.Setup(u => u.ShopItemRepository.GetCartItem(userId, itemId))
                .ReturnsAsync(new ShopItem());

        _detailsMapperMock.Setup(m => m.Map(It.IsAny<ShopItem>()))
                          .Returns(shopItemDetails);

        // Act
        var result = await _shopItemService.GetShopItem(userId, itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(shopItemDetails, result);
    }

    [Fact]
    public async Task GetCartItems_ReturnsShopItemListElements_WhenCartItemsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var cartItems = new List<ShoppingCartItem>
        {
            new ShoppingCartItem
            {
                ItemId = _dataGuids.Item1Id,
                Item = new ShopItem
                {
                    Id = _dataGuids.Item1Id,
                    Name = "Sneakers",
                    Rating = 4.5,
                    StockAmount = 10,
                    Price = 50,
                },
                Quantity = 2
            }
        };
        
        var shopItemListElement = new ShopItemListElement
        {
            Id = _dataGuids.Item1Id,
            Name = "Sneakers",
            Rating = 4.5,
            StockAmount = 10,
            Price = 50,
            QuantityTaken = 2
        };
        
        _uowMock.Setup(u => u.ShopItemRepository.GetCartItems(userId))
                .ReturnsAsync(cartItems);

        _listMapperMock.Setup(m => m.MapToShopItemListElem(It.IsAny<ShopItem>(), userId))
                       .Returns(shopItemListElement);

        // Act
        var result = await _shopItemService.GetCartItems(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(shopItemListElement, result.First());
    }

    [Fact]
    public async Task AddRemoveCartItem_CallsRepositoryMethodsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var itemId = _dataGuids.Item1Id;

        // Act
        await _shopItemService.AddRemoveCartItem(userId, itemId, ECartItemActions.Increment, null);
        await _shopItemService.AddRemoveCartItem(userId, itemId, ECartItemActions.Decrement, null);
        await _shopItemService.AddRemoveCartItem(userId, itemId, ECartItemActions.SetAmount, 5);

        // Assert
        _uowMock.Verify(u => u.ShopItemRepository.AddCartItem(userId, itemId), Times.Once);
        _uowMock.Verify(u => u.ShopItemRepository.RemoveCartItem(userId, itemId), Times.Once);
        _uowMock.Verify(u => u.ShopItemRepository.SetCartItemQuantity(userId, itemId, 5), Times.Once);
    }

    [Fact]
    public async Task RemoveAllCartItems_CallsRepositoryMethodCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _shopItemService.RemoveAllCartItems(userId);

        // Assert
        _uowMock.Verify(u => u.ShopItemRepository.RemoveAllCartItems(userId), Times.Once);
    }
}