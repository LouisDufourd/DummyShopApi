using Dapper;
using DummyShopApi.DAL.Entities;
using System.Data;
using System.Drawing;

namespace DummyShopApi.DAL.DAO.Postgrsql
{
    public class OrderDAOPostgres : IOrderDAO
    {
        private readonly ISession _db;

        public OrderDAOPostgres(ISession db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int page = 1, int size = 20)
        {
            string query = """
                select 
                    order_id as id, 
                    os.label as status,
                from orders o
                join orders_status os on os.order_status_id = o.order_status_id_fk
                limit @limit
                offset @offset;
                """;

            return await _db.Connection.QueryAsync<Order>(query, new { limit = size, offset = page * size - size });
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            string query = """
                select
                    order_id as id,
                    os.label as status
                from orders o
                join orders_status os on os.order_status_id = o.order_status_id_fk
                where order_id = @id;
                """;

            return (await _db.Connection.QueryAsync<Order>(query, new { id })).Single();
        }

        public async Task<Dictionary<Product, OrderProductStatus>> GetProductsAsync(int id, int page = 1, int size = 20)
        {
            string query = """
                select
                    product_id as id,
                    name,
                    price,
                    op.quantity,
                    description,
                    status
                from orders_products op
                join products p on p.product_id = op.product_id_fk
                where order_id_fk = @id
                """;

            var productStatus = new Dictionary<int, OrderProductStatus>();
            var products = await _db.Connection.QueryAsync<Product, OrderProductStatus, Product>(query, 
                (product, status) =>
                {
                    productStatus.Add(product.Id, status);
                    return product;
                },
                new { id },
                splitOn: "status");

            return products
                .Select(p => new KeyValuePair<Product, OrderProductStatus>(p, productStatus[p.Id]))
                .Skip(page * size - size)
                .Take(size)
                .ToDictionary();
        }

        public async Task<Order> PatchOrderStatusAsync(int id, string status)
        {
            string query = """
                update orders 
                set order_status_id = (
                    select 
                        order_status_id 
                    from orders_status 
                    where label = @status
                    limit 1
                )
                where order_id = @id
                """;

            await _db.Connection.ExecuteAsync(query, new { status, id });

            return await GetByIdAsync(id);
        }
    }
}
