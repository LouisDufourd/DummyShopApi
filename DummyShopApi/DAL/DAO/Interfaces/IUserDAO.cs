using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO.Interfaces
{
    public interface IUserDAO
    {
        public Task<bool> Login(string username, string password);
        public Task<User> GetUserByUsername(string username);
    }
}
