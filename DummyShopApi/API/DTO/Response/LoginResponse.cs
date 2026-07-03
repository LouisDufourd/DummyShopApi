namespace DummyShopApi.API.DTO.Response
{
    public class LoginResponse
    {
        public string JWTToken { get; set; }
        public string Role { get; set; }

        public LoginResponse(string jWTToken, string role)
        {
            JWTToken = jWTToken;
            Role = role;
        }
    }
}
