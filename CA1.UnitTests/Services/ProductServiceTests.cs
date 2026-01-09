using CA1.Application.DTOs;
using CA1.Application.Interfaces;
using CA1.Application.Services;
using CA1.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CA1.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<AutoMapper.IMapper> _mapperMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<AutoMapper.IMapper>();
        _productService = new ProductService(_productRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnListOfProductDtos()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1000 },
            new Product { Id = 2, Name = "Mouse", Price = 50 }
        };

        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Laptop", Price = 1000 },
            new ProductDto { Id = 2, Name = "Mouse", Price = 50 }
        };

        _productRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        _mapperMock.Setup(x => x.Map<IEnumerable<ProductDto>>(products))
            .Returns(productDtos);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task CreateAsync_ShouldCallAddAsync()
    {
        // Arrange
        var productDto = new ProductDto { Name = "New Product", Description = "Desc", Price = 100 };
        var product = new Product { Name = "New Product", Description = "Desc", Price = 100 };

        _mapperMock.Setup(x => x.Map<Product>(productDto))
            .Returns(product);

        // Act
        await _productService.CreateAsync(productDto);

        // Assert
        _productRepositoryMock.Verify(x => x.AddAsync(It.Is<Product>(p =>
            p.Name == productDto.Name &&
            p.Price == productDto.Price
        )), Times.Once);
    }
}
