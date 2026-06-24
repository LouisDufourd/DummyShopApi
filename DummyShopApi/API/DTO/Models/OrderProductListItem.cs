namespace DummyShopApi.API.DTO.Models
{
    public class OrderProductListItem
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public OrderProductListItem(int id, string status)
        {
            Id = id;
            Status = status;
        }
    }
}
