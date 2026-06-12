using Dapper;
using DummyShopApi.DAL.Entities;
using Npgsql;
using System.Runtime.CompilerServices;

namespace DummyShopApi.DAL.DAO.Postgrsql
{
    public class InventoryDAOPostgres: IInventoryDAO
    {

        private readonly ISession _db;

        public InventoryDAOPostgres(ISession session)
        {
            _db = session;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            string select = "select product_id as id, name, description from products";

            IEnumerable<Product> products = _db.Connection.Query<Product>(select);
            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
