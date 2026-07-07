using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO.Interfaces
{
    /// <summary>
    /// Provides generic asynchronous write operations for an entity.
    /// </summary>
    /// <typeparam name="T">The entity type handled by the DAO.</typeparam>
    /// <typeparam name="U">The type of the entity identifier.</typeparam>
    public interface IGenericWriteDAO<T, U> where T : IEntity
    {
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The created entity.</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity containing the updated values.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(U id);
    }
}