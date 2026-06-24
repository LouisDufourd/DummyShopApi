using Dapper;
using DummyShopApi.DAL.Entities;
using Npgsql;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace DummyShopApi.DAL.DAO.Postgrsql
{
    public class InventoryDAOPostgres : IInventoryDAO
    {
        private readonly ISession _db;

        public InventoryDAOPostgres(ISession session)
        {
            _db = session;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int page = 1, int size = 20)
        {
            string query = """
                select 
                    product_id as id, 
                    p.name, 
                    p.description,
                    c.category_id as CategoryId,
                    c.name,
                    c.description
                from products p
                join products_categories pc on pc.product_id_fk = p.product_id
                join categories c on pc.category_id_fk = c.category_id
                """;

            IEnumerable<Product> products = await _db.Connection.QueryAsync<Product, Category, Product>(
                query,
                param: new { offset = (page - 1) * size, size },
                map: (product, category) =>
                {   
                    if(category != null)
                    {
                        product.Categories = new List<Category> { category };
                    }

                    return product;
                },
                splitOn: "CategoryId"
            );

            products = products
                .GroupBy(p => p.Id)
                .Select(g =>
                {
                    Product groupedProduct = g.First();
                    groupedProduct.Categories = g.Select(p => p.Categories.Single()).ToList();
                    return groupedProduct;
                })
                .Skip(page * size - size)
                .Take(size)
                .ToList();

            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            string query = """
                select product_id as id, p.name, p.description, price, quantity, c.category_id as categoryid, c.name, c.description
                from products p 
                join products_categories pc on pc.product_id_fk = p.product_id
                join categories c on pc.category_id_fk = c.category_id
                where product_id = @id;
                """;

            IEnumerable<Product> products = await _db.Connection.QueryAsync<Product, Category, Product>(
                query,
                (product, category) =>
                {
                    if (category != null)
                        product.Categories = [category];
                    return product;
                },
                new { id },
                splitOn: "categoryid"
            );

            var newProducts = products
                .GroupBy(p => p.Id)
                .Select(g =>
                {
                    Product groupedProduct = g.First();
                    groupedProduct.Categories = g.Select(p => p.Categories.Single()).ToList();
                    return groupedProduct;
                }).ToList();

            return newProducts.First();
        }

        public async Task<Product> PatchQuantityAsync(int id, int quantity)
        {
            string query = """
                update products
                set quantity = @quantity
                where product_id = @id
                """;

            await _db.Connection.ExecuteAsync(query, new { id, quantity });

            return await GetByIdAsync(id);
        }
    }
}
