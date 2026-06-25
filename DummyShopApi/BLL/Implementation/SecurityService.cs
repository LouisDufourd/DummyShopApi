using DummyShopApi.BLL.Interface;
using DummyShopApi.DAL;
using DummyShopApi.DAL.Entities;
using DummyShopApi.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DummyShopApi.BLL.Implementation
{
    public class SecurityService : ISecurityService
    {
        private readonly IConfiguration _configuration;
        private readonly IUOW _db;

        public SecurityService(IConfiguration configuration, IUOW db)
        {
            _configuration = configuration;
            _db = db;
        }

        public async Task<string> SignIn(string username, string password)
        {
            if(!await _db.User.Login(username, password))
            {
                throw new InvalidLoginException("The username or password is incorrect");
            }

            var user = await _db.User.GetUserByUsername(username);

            return GenerateJwtToken(username, user.role);
        }

        private string GenerateJwtToken(string username, ERole role)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],//Issuer
                _configuration["JwtIssuer"],//Audience
                claims,//Charge utile (Payload)
                expires: expires, //Expiration time
                signingCredentials: creds //Signing Key
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
