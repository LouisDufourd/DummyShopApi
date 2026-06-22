using DummyShopApi.BLL.Models;
using DummyShopApi.DAL.Entities;

namespace DummyShopApi.BLL.Interfaces
{
    public interface IEcomService
    {
        Task<IEnumerable<Order>> GetOrdersAsync(int page = 1, int size = 20);
        Task<Order> GetCommandAsync(int id);
        Task<Order> UpdateOrderStatusAsync(int id, string status);
        Task<IEnumerable<Product>> GetProductsAsync(int page = 1, int size = 20);
        Task<Product> GetProductAsync(int id);
        Task<Product> UpdateProductQuantityAsync(int id, int quantity);
        Task<IEnumerable<OrderProduct>> GetOrderProductsAsync(int id, int page = 1, int size = 20);
    }
}
