using DummyShopApi.DAL.Entities;

namespace DummyShopApi.API.DTO.Response
{
    public class GetOrderProductsResponse
    {
        public Dictionary<int, string> Products { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }

        public GetOrderProductsResponse(Dictionary<int, string> products, int page, int size)
        {
            Products = products;
            Page = page;
            Size = size;
        }
    }
}
