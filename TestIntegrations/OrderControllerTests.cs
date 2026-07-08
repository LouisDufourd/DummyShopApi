using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
using DummyShopApi.DAL.Entities;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TestIntegrations.Fixtures;

namespace TestIntegrations
{
    public class OrderControllerTests : AbstractIntegrationTest
    {
        public OrderControllerTests(APIWebApplicationFactory fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task GetOrders_ShouldReturnFiveOrders()
        {
            ResetDatabase();
            Login();

            var response = await _client.GetStringAsync("/api/order");

            var result = JsonSerializer.Deserialize<GetOrdersResponse>(response, _jsonOptions);

            Assert.NotNull(result);
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.Size);
            Assert.Equal(5, result.Orders.Count);

            Assert.Equal(1, result.Orders[0].Id);
            Assert.Equal("Payée", result.Orders[0].Status);

            Assert.Equal(2, result.Orders[1].Id);
            Assert.Equal("Expédiée", result.Orders[1].Status);

            Assert.Equal(3, result.Orders[2].Id);
            Assert.Equal("Livrée", result.Orders[2].Status);

            Assert.Equal(4, result.Orders[3].Id);
            Assert.Equal("Annulée", result.Orders[3].Status);

            Assert.Equal(5, result.Orders[4].Id);
            Assert.Equal("Payée", result.Orders[4].Status);

            Logout();
        }

        [Fact]
        public async Task GetOrderProducts_ShouldReturnProducts()
        {
            ResetDatabase();
            Login();

            var response = await _client.GetStringAsync("/api/order/1/products");

            var result = JsonSerializer.Deserialize<GetOrderProductsResponse>(response, _jsonOptions);

            Assert.NotNull(result);
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.Size);

            Assert.Equal(2, result.Products.Count);

            Assert.Equal(1, result.Products[0].Id);
            Assert.Equal("Picked", result.Products[0].Status);

            Assert.Equal(4, result.Products[1].Id);
            Assert.Equal("Packed", result.Products[1].Status);

            Logout();
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnSuccess()
        {
            ResetDatabase();
            Login();

            var request = new UpdateOrderStatusRequest()
            {
                Id = 1,
                Status = "Expédiée"
            };

            var response = await _client.PutAsJsonAsync(
                "/api/order/status",
                request);

            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            Logout();
        }

        [Fact]
        public async Task UpdateProductStatus_ShouldReturnSuccess()
        {
            ResetDatabase();
            Login();

            var request = new UpdateProductStatusRequest()
            {
                ProductId = 1,
                Status = "Packed"
            };

            var response = await _client.PutAsJsonAsync(
                "/api/order/1/products/status",
                request);

            response.EnsureSuccessStatusCode();

            Logout();
        }

        [Fact]
        public async Task GetOrders_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ResetDatabase();
            Logout();

            var response = await _client.GetAsync("/api/order");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetOrderProducts_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            Logout();

            var response = await _client.GetAsync("/api/order/1/products");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateOrderStatus_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            Logout();

            var response = await _client.PutAsJsonAsync(
                "/api/order/status",
                new UpdateOrderStatusRequest() { Id = 1, Status = "Expédiée" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProductStatus_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            Logout();

            var response = await _client.PutAsJsonAsync(
                "/api/order/1/products/status",
                new UpdateProductStatusRequest() { ProductId = 1, Status = "Packed" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProducts_WithManagerRole_ReturnsProducts()
        {
            ResetDatabase();
            Login();

            var response = await _client.GetAsync("/api/Order/products");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content
                .ReadFromJsonAsync<GetOrdersProductsResponse>();

            Assert.NotNull(result);
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.Size);
            Assert.NotNull(result.Products);
        }

        [Fact]
        public async Task GetProducts_WithInventoryRole_ReturnsProducts()
        {
            ResetDatabase();
            Login();

            var response = await _client.GetAsync("/api/order/products");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Logout();
        }

        [Fact]
        public async Task GetProducts_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/order/products");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProducts_WithInvalidRole_ReturnsForbidden()
        {
            ResetDatabase();
            Login(ERole.Packer);

            var response = await _client.GetAsync("/api/order/products");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

            Logout();
        }

    }
}