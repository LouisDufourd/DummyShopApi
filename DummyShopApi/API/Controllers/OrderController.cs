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
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IEcomService _service;
        public OrderController(IEcomService ecomService)
        {
            _service = ecomService;
        }

        /// <summary>
        /// Retrieve a paginated list of orders
        /// </summary>
        /// <param name="page">The page number to retrieve</param>
        /// <param name="status">Optional status filter for the orders</param>
        /// <returns>A paginated list of orders</returns>
        /// <response code="200">Returns the list of orders</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have the required role</response>
        [HttpGet]
        [Authorize(Roles = "Manager, Packer")]
        [ProducesResponseType(typeof(GetOrdersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        /// <summary>
        /// Retrieve a paginated list of products from an order
        /// </summary>
        /// <param name="id">The order identifier</param>
        /// <param name="page">The page number to retrieve</param>
        /// <param name="status">Optional status filter for the order products</param>
        /// <returns>A paginated list of products contained in the order</returns>
        /// <response code="200">Returns the list of products in the order</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have the required role</response>
        /// <response code="404">If the order was not found</response>
        [HttpGet("{id}/products")]
        [Authorize(Roles = "Manager, Packer")]
        [ProducesResponseType(typeof(GetOrderProductsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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

        [HttpGet("products")]
        [Authorize(Roles = "Manager, Inventory")]
        [ProducesResponseType(typeof(GetOrdersProductsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1)
        {
            int size = 20;
            var products = await _service.GetOrdersProductsAsync(page, size, "none");

            var response = new GetOrdersProductsResponse(products.Select(p => new OrdersProductsItem(p.Id, p.Name, p.Quantity)).ToList(), page, size);

            return Ok(response);
        }

        /// <summary>
        /// Update the status of an order
        /// </summary>
        /// <param name="updateOrderStatusRequest">The new status information for the order</param>
        /// <returns>The updated order</returns>
        /// <response code="200">Returns the updated order</response>
        /// <response code="400">If the request validation failed</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have the required role</response>
        /// <response code="404">If the order was not found</response>
        [HttpPut("status")]
        [Authorize(Roles = "Manager, Packer")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            var order = await _service.PatchOrderStatusAsync(updateOrderStatusRequest.Id, updateOrderStatusRequest.Status);

            return Ok(order);
        }

        /// <summary>
        /// Update the status of a product inside an order
        /// </summary>
        /// <param name="updateProductStatus">The new status information for the product</param>
        /// <param name="id">The order identifier</param>
        /// <returns>An empty response when the update succeeds</returns>
        /// <response code="200">If the product status was updated successfully</response>
        /// <response code="400">If the request validation failed</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have the required role</response>
        /// <response code="404">If the order or product was not found</response>
        [HttpPut("{id}/products/status")]
        [Authorize(Roles = "Manager, Packer, Inventory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProductStatus([FromBody] UpdateProductStatusRequest updateProductStatus, [FromRoute] int id)
        {
            await _service.PatchProductStatus(updateProductStatus.ProductId, orderId: id, updateProductStatus.Status);
            return Ok();
        }
    }
}