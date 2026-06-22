using DummyShopApi.DAL.Entities;
using System.Collections;

namespace DummyShopApi.DAL.DAO
{
    public interface IOrderDAO: IGenericReadDAO<Order, int>
    {
        public Task<Dictionary<Product, OrderProductStatus>> GetProductsAsync(int id, int page = 1, int size = 20);
        public Task<Order> PatchOrderStatusAsync(int id, string status);
    }
}
