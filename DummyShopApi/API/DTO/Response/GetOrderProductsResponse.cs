using DummyShopApi.API.DTO.Models;
using DummyShopApi.DAL.Entities;

namespace DummyShopApi.API.DTO.Response
{
    public class GetOrderProductsResponse
    {
        public List<OrderProductListItem> Products { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }

        public GetOrderProductsResponse(List<OrderProductListItem> products, int page, int size)
        {
            Products = products;
            Page = page;
            Size = size;
        }
    }
}
