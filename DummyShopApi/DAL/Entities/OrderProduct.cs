namespace DummyShopApi.DAL.Entities
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public EOrderProductStatus Status { get; set; }
    }
}
