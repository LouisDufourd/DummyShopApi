using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO
{
    public interface IGenericReadRepository<T, U> where T : IEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(U id);
    }
}
