using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
using DummyShopApi.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DummyShopApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        ISecurityService _security;

        public AuthentificationController(ISecurityService security)
        {
            _security = security;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var key = await _security.SignIn(username: request.Username, password: request.Password);

            return Ok(new LoginResponse(key));
        }

        [HttpPost("loginSwagger")]
        public async Task<IActionResult> LoginSwagger([FromForm] LoginRequest request)
        {
            var key = await _security.SignIn(password: request.Password, username: request.Username);

            return Ok(new { access_token = key });
        }
    }
}
