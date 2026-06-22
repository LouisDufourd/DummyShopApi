namespace DummyShopApi.API.DTO.Models
{
    public class OrderListItem
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public OrderListItem(int id, string status)
        {
            Id = id;
            Status = status;
        }
    }
}
