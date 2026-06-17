using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO
{
    public interface IGenericReadDAO<T, U> where T : IEntity
    {
        Task<IEnumerable<T>> GetAllAsync(int page = 1, int size = 20);
        Task<T> GetByIdAsync(U id);
    }
}
