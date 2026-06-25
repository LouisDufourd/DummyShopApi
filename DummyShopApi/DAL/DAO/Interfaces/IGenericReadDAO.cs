using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO.Interfaces
{
    public interface IGenericReadDAO<T, U> where T : IEntity
    {
        /// <summary>
        /// Retrieves a paginated collection of entities.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of entities per page.</param>
        /// <returns>A collection of entities.</returns>
        Task<IEnumerable<T>> GetAllAsync(int page = 1, int size = 20);

        /// <summary>
        /// Retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The requested entity.</returns>
        /// <exception cref="NotFoundEntityException">
        /// Thrown when no entity with the specified identifier exists.
        /// </exception>
        Task<T> GetByIdAsync(U id);
    }
}
