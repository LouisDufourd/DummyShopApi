namespace DummyShopApi.API.DTO.Response
{
    public record GetOrdersProductsResponse
    {
        public List<OrdersProductsItem> Products { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }

        public GetOrdersProductsResponse(List<OrdersProductsItem> products, int page, int size)
        {
            Products = products;
            Page = page;
            Size = size;
        }
    }
}
