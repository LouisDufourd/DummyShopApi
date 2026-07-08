using DummyShopApi.DAL.Entities;

namespace DummyShopApi.BLL.Interfaces
{
    /// <summary>
    /// Provides methods for managing e-commerce operations.
    /// </summary>
    public interface IEcomService
    {
        /// <summary>
        /// Retrieves a paginated list of orders.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of orders per page.</param>
        /// <param name="status">Optional status filter for the orders.</param>
        /// <returns>A collection of orders.</returns>
        Task<IEnumerable<Order>> GetOrdersAsync(int page = 1, int size = 20, string? status = null);

        /// <summary>
        /// Retrieves an order by its identifier.
        /// </summary>
        /// <param name="id">The order identifier.</param>
        /// <returns>The matching order.</returns>
        Task<Order> GetOrderAsync(int id);

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="id">The order identifier.</param>
        /// <param name="status">The new order status.</param>
        /// <returns>The updated order.</returns>
        Task<Order> PatchOrderStatusAsync(int id, string status);

        /// <summary>
        /// Retrieves a paginated list of products.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of products per page.</param>
        /// <returns>A collection of products.</returns>
        Task<IEnumerable<Product>> GetProductsAsync(int page = 1, int size = 20);

        /// <summary>
        /// Retrieves a product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>The matching product.</returns>
        Task<Product> GetProductAsync(int id);

        /// <summary>
        /// Updates the available quantity of a product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="quantity">The new product quantity.</param>
        /// <returns>The updated product.</returns>
        Task<Product> PatchProductQuantityAsync(int productId, int quantity);

        /// <summary>
        /// Retrieves products associated with an order.
        /// </summary>
        /// <param name="id">The order identifier.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of products per page.</param>
        /// <param name="status">Optional status filter for the order products.</param>
        /// <returns>A collection of order products.</returns>
        Task<IEnumerable<OrderProduct>> GetOrderProductsAsync(int id, int page = 1, int size = 20, string? status = null);

        /// <summary>
        /// Updates the status of a product inside an order.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="status">The new product order status.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        Task PatchProductStatus(int productId, int orderId, string status);

        /// <summary>
        /// Retrieves a user by username.
        /// </summary>
        /// <param name="username">The username used to identify the user.</param>
        /// <returns>The matching user.</returns>
        Task<User> GetUser(string username);

        /// <summary>
        /// Retreives ordered products
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The size of the page</param>
        /// <param name="status">The status of the products</param>
        /// <returns>The ordered products</returns>
        Task<IEnumerable<Product>> GetOrdersProductsAsync(int page = 1, int size = 20, string status = "none");
    }
}