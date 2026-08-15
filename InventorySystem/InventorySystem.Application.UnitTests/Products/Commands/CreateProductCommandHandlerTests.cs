using FluentAssertions;
using InventorySystem.Application.Data;
using InventorySystem.Application.Products.Commands.Create;
using InventorySystem.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace InventorySystem.Application.UnitTests.Products.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new CreateProductCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenProviderDoesNotExist()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Monitor", "4K", "SKU-1", Guid.NewGuid(), "L001", 100, 10, null);

        var providers = new List<Provider>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Providers).Returns(providers.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Provider.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenSkuAlreadyExists()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "Monitor", "4K", "SKU-1", providerId, "L001", 100, 10, null);

        var providers = new List<Provider> { new Provider { Id = providerId } }.AsQueryable().BuildMockDbSet();
        var products = new List<Product> { new Product { SKU = "SKU-1" } }.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Providers).Returns(providers.Object);
        _contextMock.Setup(c => c.Products).Returns(products.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.DuplicateSKU");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessAndProductId_WhenDataIsValid()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "Monitor", "4K", "SKU-2", providerId, "L001", 100, 10, null);

        var providers = new List<Provider> { new Provider { Id = providerId } }.AsQueryable().BuildMockDbSet();
        var products = new List<Product>().AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Providers).Returns(providers.Object);
        _contextMock.Setup(c => c.Products).Returns(products.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _contextMock.Verify(c => c.Products.Add(It.IsAny<Product>()), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
