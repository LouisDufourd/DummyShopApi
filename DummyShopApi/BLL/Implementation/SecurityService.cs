using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL;
using DummyShopApi.DAL.Entities;
using DummyShopApi.Domain.Exceptions;
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

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],//Issuer
                _configuration["Jwt:Issuer"],//Audience
                claims,//Charge utile (Payload)
                expires: expires, //Expiration time
                signingCredentials: creds //Signing Key
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
