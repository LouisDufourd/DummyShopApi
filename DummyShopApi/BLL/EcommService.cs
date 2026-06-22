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

        public Task<Order> GetCommandAsync(int id)
        {
            return _db.Order.GetByIdAsync(id);
        }

        public Task<IEnumerable<Order>> GetCommandsAsync(int page)
        {
            return _db.Order.GetAllAsync(page: page);
        }

        public Task<IEnumerable<Product>> GetProductsAsync(int page = 1, int size = 20)
        {
            return _db.Inventory.GetAllAsync(page: page, size: size);
        }

        public Task<Product> GetProductAsync(int id)
        {
            return _db.Inventory.GetByIdAsync(id);
        }

        public Task<Order> UpdateCommandStatusAsync(int id, string status)
        {
            return _db.Order.PatchOrderStatusAsync(id, status);
        }

        public Task<Product> UpdateProductQuantityAsync(int id, int quantity)
        {
            return _db.Inventory.PatchQuantityAsync(id, quantity);
        }
    }
}
