using DummyShopApi.API.DTO.Models;
using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL;
using DummyShopApi.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace DummyShopApi.BLL.Implementation
{
    public class EcommService : IEcomService
    {
        private readonly IUOW _db;
        public EcommService(IUOW db) 
        { 
            _db = db;
        }

        /// <inheritdoc/>
        public Task<Order> GetOrderAsync(int id)
        {
            return _db.Order.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Product>> GetProductsAsync(int page = 1, int size = 20)
        {
            return _db.Inventory.GetAllAsync(page: page, size: size);
        }

        /// <inheritdoc/>
        public Task<Product> GetProductAsync(int id)
        {
            return _db.Inventory.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Order> PatchOrderStatusAsync(int id, string status)
        {
            _db.BeginTransaction();
            var order = await _db.Order.PatchOrderStatusAsync(id, status);
            _db.Commit();
            return order;
        }

        /// <inheritdoc/>
        public async Task<Product> PatchProductQuantityAsync(int productId, int quantity)
        {
            _db.BeginTransaction();
            var product = await _db.Inventory.PatchQuantityAsync(productId, quantity);
            _db.Commit();
            return product;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Order>> GetOrdersAsync(int page = 1, int size = 20, string? status = null)
        {
            return _db.Order.GetAllAsync(page, size, status);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrderProduct>> GetOrderProductsAsync(int id, int page = 1, int size = 20, string? status = null)
        {
            return await _db.Order.GetProductsAsync(id, page, size, GetOrderProductStatusFromNullableString(status));
        }

        /// <inheritdoc/>
        public async Task PatchProductStatus(int productId, int orderId, string status)
        {
            var enumStatus = GetOrderProductStatusFromNullableString(status);

            if (enumStatus == null)
            {
                throw new ValidationException("Unable to convert status to an enumerable");
            }

            _db.BeginTransaction();
            await _db.Order.PatchProductStatusAsync(productId: productId, orderId: orderId, status: enumStatus.Value);
            _db.Commit();
        }

        /// <inheritdoc/>
        public async Task<User> GetUser(string username)
        {
            return await _db.User.GetUserByUsername(username);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetOrdersProductsAsync(int page = 1, int size = 20, string status = "none")
        {
            return await _db.Order.GetAllProductsAsync(page, size, GetOrderProductStatusFromString(status));
        }

        /// <summary>
        /// Converts a nullable string representation of an order product status into its corresponding enum value.
        /// </summary>
        /// <param name="status">
        /// The string value representing the order product status, or null if no status filter is provided.
        /// </param>
        /// <returns>
        /// The matching <see cref="EOrderProductStatus"/> value, or null when no status is provided.
        /// </returns>
        private static EOrderProductStatus? GetOrderProductStatusFromNullableString(string? status)
        {
            EOrderProductStatus? enumStatus;
            switch (status?.ToLower())
            {
                case null:
                    enumStatus = null;
                    break;
                default:
                    enumStatus = GetOrderProductStatusFromString(status);
                    break;
            }

            return enumStatus;
        }

        /// <summary>
        /// Converts a non-null string representation of an order product status into its corresponding enum value.
        /// </summary>
        /// <param name="status">The string value representing the order product status.</param>
        /// <returns>The matching <see cref="EOrderProductStatus"/> value.</returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when the provided status does not match an existing order product status.
        /// </exception>
        private static EOrderProductStatus GetOrderProductStatusFromString(string status)
        {
            EOrderProductStatus enumStatus;
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
                default:
                    throw new NotImplementedException();
            }

            return enumStatus;
        }
    }
}
