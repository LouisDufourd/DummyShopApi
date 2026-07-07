using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO.Interfaces
{
    /// <summary>
    /// Provides asynchronous data access operations for users.
    /// </summary>
    public interface IUserDAO
    {
        /// <summary>
        /// Checks if the provided user credentials are valid.
        /// </summary>
        /// <param name="username">The username used for authentication.</param>
        /// <param name="password">The password used for authentication.</param>
        /// <returns>
        /// True if the credentials are valid; otherwise, false.
        /// </returns>
        public Task<bool> Login(string username, string password);

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username used to identify the user.</param>
        /// <returns>The matching user.</returns>
        public Task<User> GetUserByUsername(string username);
    }
}