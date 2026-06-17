using DummyShopApi.DAL.Entities;

namespace DummyShopApi.BLL.Interfaces
{
    public interface IEcomService
    {
        Task<IEnumerable<Order>> GetCommandsAsync(int page);
        Task<Order> GetCommandAsync(int id);
        Task<Order> UpdateCommandAsync(Order command);
        Task<IEnumerable<Product>> GetProductsAsync(int page);
        Task<Product> GetProductDetail(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
    }
}
