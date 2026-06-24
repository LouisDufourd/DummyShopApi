using DummyShopApi.DAL.Entities;
using System.Collections;

namespace DummyShopApi.DAL.DAO
{
    public interface IOrderDAO
    {
        public Task<IEnumerable<Order>> GetAllAsync(int page = 1, int size = 20, string? status = null);
        public Task<Order> GetByIdAsync(int id);
        public Task<IEnumerable<OrderProduct>> GetProductsAsync(int id, int page = 1, int size = 20, EOrderProductStatus? status = null);
        public Task<Order> PatchOrderStatusAsync(int id, string status);

        /// <summary>
        /// Change the status of the product
        /// </summary>
        /// <param name="id">The id of the product</param>
        /// <param name="status">The satus of the product (ToPicked, Picked, Packed)</param>
        /// <returns></returns>
        public Task PatchProductStatusAsync(int orderId, int productId, EOrderProductStatus status);
    }
}
