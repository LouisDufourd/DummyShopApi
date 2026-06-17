using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL;
using DummyShopApi.DAL.Entities;

namespace DummyShopApi.BLL
{
    public class EcommService : IEcomService
    {
        private readonly IUOW _db;
        public EcommService(IUOW db) 
        { 
            _db = db;
        }

        public Task<Product> AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetCommandAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Order>> GetCommandsAsync(int page)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int page)
        {
            return await _db.Inventory.GetAllAsync(page);
        }

        public async Task<Product> GetProductDetail(int id)
        {
            var product = await _db.Inventory.GetByIdAsync(id);

            return product;
        }

        public Task<Order> UpdateCommandAsync(Order command)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
