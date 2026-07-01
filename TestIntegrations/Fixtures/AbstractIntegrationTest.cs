using Dapper;
using DummyShopApi.DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TestIntegrations.Fixtures
{
    public class AbstractIntegrationTest : IClassFixture<APIWebApplicationFactory>
    {
        protected readonly HttpClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;

        private readonly string _issuer;
        private readonly string _key;
        private readonly string _expire;
        private readonly string _connectionString;

        public AbstractIntegrationTest(APIWebApplicationFactory fixture)
        {
            _client = fixture.CreateClient();

            _connectionString = fixture.Configuration["ConnectionString"]!;
            _issuer = fixture.Configuration["Jwt:Issuer"]!;
            _key = fixture.Configuration["Jwt:Key"]!;
            _expire = fixture.Configuration["Jwt:ExpireDays"]!;
            _jsonOptions = new()
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public void ResetDatabase()
        {
            Debug.WriteLine(Directory.GetCurrentDirectory());
            var sql = File.ReadAllText("../../../initdb.d/ECommerceInitTest.sql");

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void Login()
        {

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateJwtToken("Test", ERole.Manager));
        }

        public void Logout()
        {
            _client.DefaultRequestHeaders.Authorization = null;
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

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_key!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(Convert.ToDouble(_expire));

            var token = new JwtSecurityToken(
                _issuer,//Issuer
                _issuer,//Audience
                claims,//Charge utile (Payload)
                expires: expires, //Expiration time
                signingCredentials: creds //Signing Key
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
