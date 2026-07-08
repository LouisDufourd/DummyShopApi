using Dapper;
using DummyShopApi.DAL.DAO.Interfaces;
using DummyShopApi.DAL.Entities;
using DummyShopApi.Domain.Exceptions;
using System.Data;
using System.Drawing;
using System.Text;

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

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int page, int size, EOrderProductStatus status)
        {
            string query = """
                select
                    p.product_id as id,
                    p.name,
                    sum(op.quantity) as quantity
                from orders_products op
                join products p on p.product_id = op.product_id_fk
                where op.status = @status::product_order_status
                group by p.product_id, p.name
                order by p.product_id
                limit @limit
                offset @offset;
                """;

            return await _db.Connection.QueryAsync<Product>(query, new { limit = size, offset = page * size - size, status = status.ToString().ToLower() });
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

            var order = await _db.Connection.QuerySingleAsync<Order>(query, new { id });

            if (order == null)
            {
                throw new NotFoundEntityException("Unable to find entity with the specified id");
            }

            return order;
        }

        public async Task<IEnumerable<OrderProduct>> GetProductsAsync(int id, int page = 1, int size = 20, EOrderProductStatus? status = null)
        {
            var query = new StringBuilder("""
                select
                    p.product_id as id,
                    p.name,
                    p.price,
                    p.quantity,
                    p.description,
                    op.status
                from orders_products op
                join products p on p.product_id = op.product_id_fk
                where order_id_fk = @id
             """);

            var parameters = new DynamicParameters();
            parameters.Add("id", id);
            parameters.Add("size", size);
            parameters.Add("offset", (page - 1) * size);

            if (status.HasValue)
            {
                query.AppendLine("and op.status = @status::product_order_status");
                parameters.Add("status", status.Value.ToString().ToLower());
            }

            query.AppendLine();
            query.AppendLine("limit @size");
            query.AppendLine("offset @offset");

            return await _db.Connection.QueryAsync<OrderProduct>(query.ToString(), parameters);
        }

        public async Task<Order> PatchOrderStatusAsync(int id, string status)
        {
            string query = """
                update orders 
                set order_status_id_fk = (
                    select 
                        order_status_id 
                    from orders_status 
                    where label = @status
                    limit 1
                )
                where order_id = @id
                """;

            int row = await _db.Connection.ExecuteAsync(query, new { status, id }, _db.Transaction);

            if (row == 0)
            {
                throw new NotFoundEntityException("Unable to update the entity with the specified id because it did not exist");
            }

            return await GetByIdAsync(id);
        }

        public async Task PatchProductStatusAsync(int productId, int orderId, EOrderProductStatus status)
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

            if (row == 0)
            {
                _db.Transaction.Rollback();
                throw new NotFoundEntityException("Could not find any product with the specified id that are in the order with the specified id");
            }
        }
    }
}
