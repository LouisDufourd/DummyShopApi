using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO
{
    public interface IInventoryDAO: IGenericReadDAO<Product, int>
    {
        /// <summary>
        /// Set the quantity of a product
        /// </summary>
        /// <param name="id">The id of the product</param>
        /// <param name="quantity">The quantity of the product</param>
        /// <returns>The updated product</returns>
        public Task<Product> PatchQuantityAsync(int id, int quantity);
    }
}
