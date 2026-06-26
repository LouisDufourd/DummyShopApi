using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO.Interfaces
{
    public interface IGenericWriteDAO<T, U> where T : IEntity
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(U id);
    }
}
