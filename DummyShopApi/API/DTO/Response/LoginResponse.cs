namespace DummyShopApi.API.DTO.Response
{
    public class LoginResponse
    {
        public string JWTToken { get; set; }

        public LoginResponse(string jWTToken)
        {
            JWTToken = jWTToken;
        }
    }
}
