using DummyShopApi.DAL.Entities;
using System.Collections;

namespace DummyShopApi.DAL.DAO
{
    public interface IOrderDAO: IGenericReadDAO<Order, int>
    {
        public Task<Dictionary<Product, bool>> GetProductsAsync(int id);
        public Task<Order> PatchOrderStatusAsync(Order order);
    }
}
