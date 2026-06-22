using DummyShopApi.API.DTO.Models;

namespace DummyShopApi.API.DTO.Response
{
    public class GetInventoryResponse
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public List<ProductListItem> Products { get; set; }

        public GetInventoryResponse(int page, int size, List<ProductListItem> products)
        {
            Page = page;
            Size = size;
            Products = products;
        }
    }
}
