namespace DummyShopApi.API.DTO.Response
{
    public class GetProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public List<string> Categories { get; set; }

        public GetProductResponse(int id, string name, string description, int quantity, decimal sellingPrice, List<string> categories)
        {
            Id = id;
            Name = name;
            Description = description;
            Quantity = quantity;
            SellingPrice = sellingPrice;
            Categories = categories;
        }
    }
}
