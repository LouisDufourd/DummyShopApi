using Dapper;
using DummyShopApi.DAL.DAO.Interfaces;
using DummyShopApi.DAL.Entities;
using DummyShopApi.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DummyShopApi.DAL.DAO.Postgrsql
{
    public class UserDAOPostgres : IUserDAO
    {
        private readonly ISession _db;

        public UserDAOPostgres(ISession db)
        {
            _db = db;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            string query = """
                select 
                    user_id as id,
                    firstname,
                    name as lastname,
                    role
                from users
                join employees on user_id_fk = user_id
                where email = @username;
                """;

            var users = await _db.Connection.QueryAsync<User>(query, new { username });

            if(users.Count() == 0)
            {
                throw new NotFoundEntityException("Unable to find user with the specidied username");
            }

            return users.Single();
        }

        public async Task<bool> Login(string username, string password)
        {
            var selectPasswordQuery = """
                select
                    password
                from users
                join employees on user_id_fk = user_id
                where email = @username
                """;
            var PH = new PasswordHasher<string>();

            var hashedPassowrds = await _db.Connection.QueryAsync<string>(selectPasswordQuery, new { username });

            if(hashedPassowrds.Count() == 0)
            {
                return false;
            }

            var hashedPassword = hashedPassowrds.Single();

            return PH.VerifyHashedPassword(username, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
