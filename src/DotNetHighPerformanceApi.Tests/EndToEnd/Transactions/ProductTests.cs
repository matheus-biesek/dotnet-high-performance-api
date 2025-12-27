using System.Net.Http.Json;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DotNetHighPerformanceApi.Tests.EndToEnd.Transactions;

public class ProductTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk_AndListProducts()
    {
        // Arrange
        // (No arrange needed, DB is seeded on startup)

        // Act
        var response = await _client.GetAsync("/v1/products");

        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK, $"Reason: {content}");

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().NotBeEmpty();
        products!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnOk_AndCorrectProduct()
    {
        // Arrange
        var productId = 1;

        // Act
        var response = await _client.GetAsync($"/v1/products/{productId}");

        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK, $"Reason: {content}");

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(productId);
        product.Name.Should().StartWith("Produto");
    }
}
