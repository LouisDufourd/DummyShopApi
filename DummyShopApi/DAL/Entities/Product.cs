using System.ComponentModel;

namespace DummyShopApi.DAL.Entities
{
    public class Product: IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
