using Dapper;
using DummyShopApi.DAL.Entities;
using DummyShopApi.Domain.Exceptions;
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

        public async Task<IEnumerable<Order>> GetAllAsync(int page = 1, int size = 20, string? status = null)
        {
            string query = """
                select 
                    order_id as id, 
                    os.label as status
                from orders o
                join orders_status os on os.order_status_id = o.order_status_id_fk
                where 
                    (@status is null or lower(os.label) = @status)
                limit @limit
                offset @offset;
                """;

            return await _db.Connection.QueryAsync<Order>(query, new { limit = size, offset = page * size - size, status = status?.ToLower() });
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

        public async Task<IEnumerable<OrderProduct>> GetProductsAsync(int id, int page = 1, int size = 20, EOrderProductStatus? status = null)
        {
            string query = """
                select
                    p.product_id as id,
                    p.name,
                    p.price,
                    p.quantity,
                    p.description,
                    op.status
                from orders_products op
                join products p on p.product_id = op.product_id_fk
                where 
                    order_id_fk = @id and
                    (@status is null or op.status = @status::product_order_status)
                limit @size
                offset @offset
                """;

            return await _db.Connection.QueryAsync<OrderProduct>(query, new { id, status = status?.ToString().ToLower(), size, offset = page * size - size });
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

        public async Task PatchProductStatusAsync(int orderId, int productId, EOrderProductStatus status)
        {
            string query = """
                update orders_products
                set status = @status::product_order_status
                where 
                    product_id_fk = @productId and
                    order_id_fk = @orderId
                """;

            int row = await _db.Connection.ExecuteAsync(
                query,
                new { orderId, productId, status = status.ToString().ToLower() },
                _db.Transaction
            );
            
            if(row == 0)
            {
                _db.Transaction.Rollback();
                throw new NotFoundEntityException("Could not find any product with the specified id that are in the order with the specified id");
            }

            if(row > 1)
            {
                _db.Transaction.Rollback();
            }
        }
    }
}
