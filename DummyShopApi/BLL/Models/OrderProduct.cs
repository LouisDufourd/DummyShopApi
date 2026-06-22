namespace DummyShopApi.BLL.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }

        public OrderProduct(int id, string name, string description, int quantity, decimal price, string status)
        {
            Id = id;
            Name = name;
            Description = description;
            Quantity = quantity;
            Price = price;
            Status = status;
        }
    }
}
