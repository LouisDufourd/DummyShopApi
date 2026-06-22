namespace DummyShopApi.API.DTO.Request
{
    public class UpdateOrderStatusRequest
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}