using DummyShopApi.API.DTO.Models;
using DummyShopApi.API.DTO.Response;
using DummyShopApi.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DummyShopApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IEcomService _service;
        public OrderController(IEcomService ecomService)
        {
            _service = ecomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int page = 1)
        {
            int size = 20;
            var orders = await _service.GetOrdersAsync(page, size);
            var orderResponses = new GetOrdersResponse(
                page,
                size,
                orders: orders.Select((o) =>
                {
                    return new OrderListItem(
                        o.Id,
                        o.Status
                    );
                }).ToList()
            );

            return Ok(orderResponses);
        }
    }
}
