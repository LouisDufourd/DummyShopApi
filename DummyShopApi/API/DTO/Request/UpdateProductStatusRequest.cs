namespace DummyShopApi.API.DTO.Request
{
    public class UpdateProductStatusRequest
    {
        public int ProductId { get; set; }
        public string Status { get; set; }
    }
}
