using DummyShopApi.API.DTO.Models;

namespace DummyShopApi.API.DTO.Response
{
    public class GetOrdersResponse
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public List<OrderListItem> Orders { get; set; }

        public GetOrdersResponse(int page, int size, List<OrderListItem> orders)
        {
            Page = page;
            Size = size;
            Orders = orders;
        }
    }
}
