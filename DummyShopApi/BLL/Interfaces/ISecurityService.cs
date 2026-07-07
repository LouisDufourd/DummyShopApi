namespace DummyShopApi.BLL.Interfaces
{
    public interface ISecurityService
    {
        /// <summary>
        /// Permet de se connecter à l'API renvoie un token
        /// </summary>
        /// <param name="username">The email of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>Token en Base64</returns>
        public Task<string> SignIn(string username, string password);
    }
}
