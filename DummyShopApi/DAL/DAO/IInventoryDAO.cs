using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO
{
    public interface IInventoryDAO : IGenericReadDAO<Product, int>
    {
        /// <summary>
        /// Updates the quantity of a product.
        /// </summary>
        /// <param name="id">The identifier of the product.</param>
        /// <param name="quantity">The new quantity of the product.</param>
        /// <returns>The updated product.</returns>
        /// <exception cref="NotFoundEntityException">
        /// Thrown when no product with the specified identifier exists.
        /// </exception>
        public Task<Product> PatchQuantityAsync(int id, int quantity);
    }
}
