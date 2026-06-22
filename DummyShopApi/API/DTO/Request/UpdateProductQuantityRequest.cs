namespace DummyShopApi.API.DTO.Request
{
    public class UpdateProductQuantityRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
