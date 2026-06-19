using Dapper;
using DummyShopApi.DAL.Entities;
using System.Data;

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

        public async Task<Dictionary<Product, bool>> GetProductsAsync(int id)
        {
            string query = """
                select
                    product_id as id,
                    name,
                    price,
                    op.quantity,
                    description,
                    is_packed as isPacked
                from orders_products op
                join products p on p.product_id = op.product_id_fk
                where order_id_fk = @id
                """;

            var isPackeds = new Dictionary<int, bool>();
            var products = await _db.Connection.QueryAsync<Product, bool, Product>(query, 
                (product, isPacked) =>
                {
                    isPackeds.Add(product.Id, isPacked);
                    return product;
                },
                new { id },
                splitOn: "isPacked");

            return products.Select(p => new KeyValuePair<Product, bool>(p, isPackeds[p.Id])).ToDictionary();
        }

        public async Task<Order> PatchOrderStatusAsync(Order order)
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

            await _db.Connection.ExecuteAsync(query, new { status = order.Status, id = order.Id });

            return await GetByIdAsync(order.Id);
        }

        public Task<Order> PatchOrderStatusAsync(int id, string status)
        {
            throw new NotImplementedException();
        }
    }
}
