using DummyShopApi.API.DTO.Models;
using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL.Entities;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderProducts([FromRoute] int id, [FromQuery] int page = 1)
        {
            int size = 20;
            var products = await _service.GetOrderProductsAsync(id, page, size);
            var response = new GetOrderProductsResponse(
                products: products.Select(p => new KeyValuePair<int, string>(p.Id, p.Status)).ToDictionary(),
                page,
                size
            );

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            var order = await _service.UpdateOrderStatusAsync(updateOrderStatusRequest.Id, updateOrderStatusRequest.Status);

            return Ok(order);
        }
    }
}