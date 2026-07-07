using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
using DummyShopApi.API.Filters;
using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DummyShopApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter<ApiExceptionFilterAttribute>]
    [Produces("application/json")]
    public class AuthentificationController : ControllerBase
    {
        private readonly ISecurityService _security;
        private readonly IEcomService _ecom;

        public AuthentificationController(ISecurityService security, IEcomService ecom)
        {
            _security = security;
            _ecom = ecom;
        }

        /// <summary>
        /// Generate a token if the credential are correct
        /// </summary>
        /// <param name="request">The credential for the authentification</param>
        /// <returns>The authorization token</returns>
        /// <response code="200">Return the token</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the credential is incorrect</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var key = await _security.SignIn(username: request.Username, password: request.Password);

            var user = await _ecom.GetUser(request.Username);

            return Ok(new LoginResponse(key, user.Role.ToString()));
        }

        /// <summary>
        /// Generate a token for Swagger authentication if the credentials are correct
        /// </summary>
        /// <param name="request">The credentials for the authentication</param>
        /// <returns>The access token used by Swagger authorization</returns>
        /// <response code="200">Returns the access token</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the credentials are incorrect</response>
        [HttpPost("loginSwagger")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginSwagger([FromForm] LoginRequest request)
        {
            var key = await _security.SignIn(password: request.Password, username: request.Username);

            return Ok(new { access_token = key });
        }
    }
}
