namespace DummyShopApi.API.DTO.Response
{
    public record OrdersProductsItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductQuantity { get; set; }

        public OrdersProductsItem(int productId, string productName, int productQuantity)
        {
            ProductId = productId;
            ProductName = productName;
            ProductQuantity = productQuantity;
        }
    }
}
