namespace DummyShopApi.API.DTO.Models
{
    public class ProductListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public List<string> Categories { get; set; }

        public ProductListItem(int id, string name, int quantity, List<string> categories)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Categories = categories;
        }
    }
}
