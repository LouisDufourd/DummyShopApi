using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DummyShopApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        private readonly ISecurityService _security;
        private readonly IEcomService _ecom;

        public AuthentificationController(ISecurityService security, IEcomService ecom)
        {
            _security = security;
            _ecom = ecom;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var key = await _security.SignIn(username: request.Username, password: request.Password);

            var user = await _ecom.GetUser(request.Username);

            return Ok(new LoginResponse(key, user.Role.ToString()));
        }

        [HttpPost("loginSwagger")]
        public async Task<IActionResult> LoginSwagger([FromForm] LoginRequest request)
        {
            var key = await _security.SignIn(password: request.Password, username: request.Username);

            return Ok(new { access_token = key });
        }
    }
}
