using DummyShopApi.API.DTO.Models;
using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
using DummyShopApi.API.Filters;
using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DummyShopApi.API.Controllers
{
    [TypeFilter<ApiExceptionFilterAttribute>]
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
        [Authorize(Roles = "Manager, Packer")]
        public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] string? status = null)
        {
            if(page < 1)
            {
                throw new NotImplementedException();
            }

            int size = 20;
            var orders = await _service.GetOrdersAsync(page, size, status);
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

        [HttpGet("{id}/products")]
        [Authorize(Roles = "Manager, Packer, Inventory")]
        public async Task<IActionResult> GetOrderProducts([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] string? status = null)
        {
            int size = 20;
            var products = await _service.GetOrderProductsAsync(id, page, size, status);
            var response = new GetOrderProductsResponse(
                products: products.Select(p => new OrderProductListItem(p.Id, p.Status.ToString())).ToList(),
                page,
                size
            );

            return Ok(response);
        }

        [HttpPut("status")]
        [Authorize(Roles = "Manager, Packer")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            var order = await _service.PatchOrderStatusAsync(updateOrderStatusRequest.Id, updateOrderStatusRequest.Status);

            return Ok(order);
        }

        [HttpPut("{id}/products/status")]
        [Authorize(Roles = "Manager, Packer, Inventory")]
        public async Task<IActionResult> UpdateProductStatus([FromBody] UpdateProductStatusRequest updateProductStatus, [FromRoute] int id)
        {
            await _service.PatchProductStatus(updateProductStatus.ProductId, orderId: id, updateProductStatus.Status);
            return Ok();
        }
    }
}