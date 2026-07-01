using DummyShopApi.API.DTO.Models;
using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TestIntegrations.Fixtures;
using Xunit;

namespace TestIntegrations
{
    public class StockControllerTests : AbstractIntegrationTest
    {
        public StockControllerTests(APIWebApplicationFactory fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetInventory_ShouldBeEight()
        {
            ResetDatabase();
            var stock = new GetInventoryResponse(
                1,
                20,
                [
                    new(1, "Gaming Mouse", 40, ["Electronics", "Gaming"]),
                    new(2, "Mechanical Keyboard", 25, ["Computers"]),
                    new(3, "27 inch Monitor", 15, ["Electronics", "Computers"]),
                    new(4, "USB-C Cable", 300, ["Electronics"]),
                    new(5, "Office Chair", 8, ["Home"]),
                    new(6, "Notebook", 500, ["Books"]),
                    new(7, "Gaming Laptop", 3, ["Electronics", "Computers", "Gaming"]),
                    new(8, "Desk Lamp", 0, ["Home", "Office"]),
                ]);

            Login();

            var response = await _client.GetStringAsync("/api/stock/inventory");

            var result = JsonSerializer.Deserialize<GetInventoryResponse>(response, _jsonOptions);

            Assert.Equal(stock.Page, result.Page);
            Assert.Equal(stock.Size, result.Size);
            Assert.Equal(stock.Products.Count, result.Products.Count);

            for (int i = 0; i < result.Products.Count; i++)
            {
                var expectedProduct = stock.Products[i];
                var actualProduct = result.Products[i];

                Assert.Equal(expectedProduct.Id, actualProduct.Id);
                Assert.Equal(expectedProduct.Name, actualProduct.Name);
                Assert.Equal(expectedProduct.Quantity, actualProduct.Quantity);
                Assert.Equal(expectedProduct.Categories.Count, actualProduct.Categories.Count);

                for (int j = 0; j < actualProduct.Categories.Count; j++)
                {
                    Assert.Equal(expectedProduct.Categories[j], actualProduct.Categories[j]);
                }
            }

            Logout();
        }

        [Fact]
        public async Task GetProduct_ShouldReturnGamingMouse()
        {
            ResetDatabase();
            Login();

            var response = await _client.GetStringAsync("/api/stock/inventory/1");

            var result = JsonSerializer.Deserialize<GetProductResponse>(response, _jsonOptions);

            Assert.NotNull(result);

            Assert.Equal(1, result.Id);
            Assert.Equal("Gaming Mouse", result.Name);
            Assert.Equal("High precision wireless mouse", result.Description);
            Assert.Equal(40, result.Quantity);
            Assert.Equal(79.99m, result.SellingPrice);

            Assert.Equal(2, result.Categories.Count);
            Assert.Contains("Electronics", result.Categories);
            Assert.Contains("Gaming", result.Categories);

            Logout();
        }

        [Fact]
        public async Task UpdateProductQuantity_ShouldUpdateQuantity()
        {
            ResetDatabase();
            Login();

            var request = new UpdateProductQuantityRequest()
            {
                Quantity = 125
            };

            var response = await _client.PutAsJsonAsync(
                "/api/stock/inventory/1/quantity",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GetProductResponse>(_jsonOptions);

            Assert.NotNull(result);

            Assert.Equal(1, result.Id);
            Assert.Equal(125, result.Quantity);

            // Verify persistence
            var verifyResponse = await _client.GetStringAsync("/api/stock/inventory/1");
            var verify = JsonSerializer.Deserialize<GetProductResponse>(verifyResponse, _jsonOptions);

            Assert.Equal(125, verify.Quantity);

            Logout();
        }

        [Fact]
        public async Task GetInventory_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            Logout();

            var response = await _client.GetAsync("/api/stock/inventory");

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProduct_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            Logout();

            var response = await _client.GetAsync("/api/stock/inventory/1");

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProductQuantity_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            Logout();

            var response = await _client.PutAsJsonAsync(
                "/api/stock/inventory/1/quantity",
                new UpdateProductQuantityRequest() { Quantity = 10 });

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}