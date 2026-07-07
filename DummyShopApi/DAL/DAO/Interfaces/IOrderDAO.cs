using DummyShopApi.DAL.Entities;
using DummyShopApi.Domain.Exceptions;
using System.Collections;

namespace DummyShopApi.DAL.DAO.Interfaces
{
    public interface IOrderDAO
    {
        /// <summary>
        /// Retrieves a paginated list of orders.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of orders per page.</param>
        /// <param name="status">Optional order status to filter by.</param>
        /// <returns>A collection of matching orders.</returns>
        public Task<IEnumerable<Order>> GetAllAsync(int page = 1, int size = 20, string? status = null);

        /// <summary>
        /// Retrieves an order by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the order.</param>
        /// <returns>The requested order.</returns>
        /// <exception cref="NotFoundEntityException">
        /// Thrown when no order with the specified identifier exists.
        /// </exception>
        public Task<Order> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a paginated list of products belonging to an order.
        /// </summary>
        /// <param name="id">The identifier of the order.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of products per page.</param>
        /// <param name="status">Optional product status to filter by.</param>
        /// <returns>A collection of products belonging to the order.</returns>
        /// <exception cref="NotFoundEntityException">
        /// Thrown when no order with the specified identifier exists.
        /// </exception>
        public Task<IEnumerable<OrderProduct>> GetProductsAsync(
            int id,
            int page = 1,
            int size = 20,
            EOrderProductStatus? status = null);

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="id">The identifier of the order.</param>
        /// <param name="status">The new status of the order.</param>
        /// <returns>The updated order.</returns>
        /// <exception cref="NotFoundEntityException">
        /// Thrown when no order with the specified identifier exists.
        /// </exception>
        public Task<Order> PatchOrderStatusAsync(int id, string status);

        /// <summary>
        /// Updates the status of a product within an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order.</param>
        /// <param name="productId">The identifier of the product.</param>
        /// <param name="status">The new product status (ToPicked, Picked, or Packed).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="NotFoundEntityException">
        /// Thrown when the specified order or product does not exist.
        /// </exception>
        public Task PatchProductStatusAsync(int productId, int orderId, EOrderProductStatus status);
    }
}
