using DummyShopApi.API.DTO.Models;
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

        public Task<IEnumerable<Product>> GetProductsAsync(int page = 1, int size = 20)
        {
            return _db.Inventory.GetAllAsync(page: page, size: size);
        }

        public Task<Product> GetProductAsync(int id)
        {
            return _db.Inventory.GetByIdAsync(id);
        }

        public Task<Order> PatchOrderStatusAsync(int id, string status)
        {
            return _db.Order.PatchOrderStatusAsync(id, status);
        }

        public Task<Product> PatchProductQuantityAsync(int id, int quantity)
        {
            return _db.Inventory.PatchQuantityAsync(id, quantity);
        }

        public Task<IEnumerable<Order>> GetOrdersAsync(int page = 1, int size = 20, string? status = null)
        {
            return _db.Order.GetAllAsync(page, size, status);
        }

        public async Task<IEnumerable<OrderProduct>> GetOrderProductsAsync(int id, int page = 1, int size = 20, string? status = null)
        {
            return await _db.Order.GetProductsAsync(id, page, size, GetOrderProductStatusFromString(status));
        }

        public async Task PatchProductStatus(int productId, int orderId, string status)
        {
            var enumStatus = GetOrderProductStatusFromString(status);

            if (enumStatus == null)
            {
                throw new Exception("Status should not be null here");
            }

            _db.BeginTransaction();
            await _db.Order.PatchProductStatusAsync(productId: productId, orderId: orderId, status: enumStatus.Value);
        }

        private EOrderProductStatus? GetOrderProductStatusFromString(string? status)
        {
            EOrderProductStatus? enumStatus;
            switch (status?.ToLower())
            {
                case "none":
                    enumStatus = EOrderProductStatus.None;
                    break;
                case "picked":
                    enumStatus = EOrderProductStatus.Picked;
                    break;
                case "packed":
                    enumStatus = EOrderProductStatus.Packed;
                    break;
                case null:
                    enumStatus = null;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return enumStatus;
        }
    }
}
