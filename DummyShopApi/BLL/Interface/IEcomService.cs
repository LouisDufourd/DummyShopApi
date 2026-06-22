using DummyShopApi.DAL.Entities;

namespace DummyShopApi.BLL.Interfaces
{
    public interface IEcomService
    {
        Task<IEnumerable<Order>> GetCommandsAsync(int page);
        Task<Order> GetCommandAsync(int id);
        Task<Order> UpdateCommandStatusAsync(int id, string status);
        Task<IEnumerable<Product>> GetProductsAsync(int page = 1, int size = 20);
        Task<Product> GetProductAsync(int id);
        Task<Product> UpdateProductQuantityAsync(int id, int quantity);
    }
}
