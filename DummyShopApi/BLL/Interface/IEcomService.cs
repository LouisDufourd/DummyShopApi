using DummyShopApi.DAL.Entities;

namespace DummyShopApi.BLL.Interfaces
{
    public interface IEcomService
    {
        Task<IEnumerable<Command>> GetCommandsAsync(int page);
        Task<Command> GetCommandAsync(int id);
        Task<Command> UpdateCommandAsync(Command command);
        Task<IEnumerable<Product>> GetProductsAsync(int page);
        Task<Product> GetProductDetail(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
    }
}
