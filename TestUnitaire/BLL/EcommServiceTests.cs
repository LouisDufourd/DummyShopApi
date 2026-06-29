using DummyShopApi.BLL.Implementation;
using DummyShopApi.DAL;
using DummyShopApi.DAL.DAO.Interfaces;
using DummyShopApi.DAL.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUnitaire.BLL
{
    public class EcommServiceTests
    {
        private readonly Mock<IUOW> _uow;
        private readonly Mock<IOrderDAO> _orderDao;
        private readonly Mock<IInventoryDAO> _inventoryDao;

        private readonly EcommService _service;

        public EcommServiceTests()
        {
            _uow = new Mock<IUOW>();
            _orderDao = new Mock<IOrderDAO>();
            _inventoryDao = new Mock<IInventoryDAO>();

            _uow.Setup(x => x.Order).Returns(_orderDao.Object);
            _uow.Setup(x => x.Inventory).Returns(_inventoryDao.Object);

            _service = new EcommService(_uow.Object);
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnOrder()
        {
            var order = new Order { Id = 1 };

            _orderDao
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(order);

            var result = await _service.GetOrderAsync(1);

            Assert.Equal(order, result);

            _orderDao.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetOrdersAsync_ShouldReturnOrders()
        {
            var orders = new List<Order>()
            {
                new(),
                new()
            };

            _orderDao
                .Setup(x => x.GetAllAsync(1, 20, null))
                .ReturnsAsync(orders);

            var result = await _service.GetOrdersAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProductAsync_ShouldReturnProduct()
        {
            var product = new Product { Id = 1 };

            _inventoryDao
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            var result = await _service.GetProductAsync(1);

            Assert.Equal(product, result);
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnProducts()
        {
            var products = new List<Product>()
            {
                new(),
                new()
            };

            _inventoryDao
                .Setup(x => x.GetAllAsync(1, 20))
                .ReturnsAsync(products);

            var result = await _service.GetProductsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task PatchOrderStatus_ShouldCommitTransaction()
        {
            var order = new Order();

            _orderDao
                .Setup(x => x.PatchOrderStatusAsync(1, "Paid"))
                .ReturnsAsync(order);

            var result = await _service.PatchOrderStatusAsync(1, "Paid");

            Assert.Equal(order, result);

            _uow.Verify(x => x.BeginTransaction(), Times.Once);

            _uow.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task PatchProductQuantity_ShouldCommitTransaction()
        {
            var product = new Product();

            _inventoryDao
                .Setup(x => x.PatchQuantityAsync(1, 10))
                .ReturnsAsync(product);

            var result = await _service.PatchProductQuantityAsync(1, 10);

            Assert.Equal(product, result);

            _uow.Verify(x => x.BeginTransaction(), Times.Once);

            _uow.Verify(x => x.Commit(), Times.Once);
        }

        [Theory]
        [InlineData("none", EOrderProductStatus.None)]
        [InlineData("picked", EOrderProductStatus.Picked)]
        [InlineData("packed", EOrderProductStatus.Packed)]
        [InlineData(null, null)]
        public async Task GetOrderProducts_ShouldConvertStatus(string status, EOrderProductStatus? expected)
        {
            var products = new List<OrderProduct>();

            _orderDao
                .Setup(x => x.GetProductsAsync(1, 1, 20, expected))
                .ReturnsAsync(products);

            await _service.GetOrderProductsAsync(1, status: status);

            _orderDao.Verify(x =>
                x.GetProductsAsync(1, 1, 20, expected),
                Times.Once);
        }

        [Fact]
        public async Task GetOrderProducts_ShouldThrow_WhenStatusIsUnknown()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() =>
                _service.GetOrderProductsAsync(1, status: "banana"));
        }

        [Fact]
        public async Task PatchProductStatus_ShouldCommitTransaction()
        {
            await _service.PatchProductStatus(1, 2, "packed");

            _uow.Verify(x => x.BeginTransaction(), Times.Once);

            _uow.Verify(x => x.Commit(), Times.Once);

            _orderDao.Verify(x =>
                x.PatchProductStatusAsync(
                    1,
                    2,
                    EOrderProductStatus.Packed),
                Times.Once);
        }

        [Fact]
        public async Task PatchProductStatus_ShouldThrow_WhenStatusIsUnknown()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() =>
                _service.PatchProductStatus(1, 2, "banana"));
        }
    }
}
