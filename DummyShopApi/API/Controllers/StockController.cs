using DummyShopApi.API.DTO.Models;
using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
using DummyShopApi.API.Filters;
using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL;
using DummyShopApi.DAL.DAO.Postgrsql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DummyShopApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter<ApiExceptionFilterAttribute>]
    [Produces("application/json")]
    public class StockController : ControllerBase
    {
        private readonly IEcomService _service;
        public StockController(IEcomService ecomService) { 
            _service = ecomService;
        }

        /// <summary>
        /// Retrieve a paginated list of products in the inventory
        /// </summary>
        /// <param name="page">The page number to retrieve</param>
        /// <returns>A paginated list of inventory products</returns>
        /// <response code="200">Returns the list of products in the inventory</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have the required role</response>
        [HttpGet("inventory")]
        [Authorize(Roles = "Manager, Inventory")]
        [ProducesResponseType(typeof(GetInventoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetInventory([FromQuery] int page = 1)
        {
            int size = 20;
            
            var inventory = (await _service.GetProductsAsync(page, size)).Select((p) => {
                return new ProductListItem(
                    p.Id, 
                    p.Name, 
                    p.Quantity, 
                    [.. p.Categories.Select((c) => c.Name)]
                );
            }).ToList();

            var inventoryResponse = new GetInventoryResponse(page: page, size: size, products: inventory);

            return Ok(inventoryResponse);
        }

        /// <summary>
        /// Retrieve a product from the inventory
        /// </summary>
        /// <param name="id">The product identifier</param>
        /// <returns>The product details</returns>
        /// <response code="200">Returns the product details</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have the required role</response>
        /// <response code="404">If the product was not found</response>
        [HttpGet("inventory/{id}")]
        [Authorize(Roles = "Manager, Inventory")]
        [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await _service.GetProductAsync(id);
            
            var productResponse = new GetProductResponse(
                id: product.Id,
                name: product.Name,
                description: product.Description,
                quantity: product.Quantity,
                sellingPrice: product.Price,
                categories: product.Categories.Select((c) => c.Name).ToList()
            );

            return Ok(productResponse);
        }

        /// <summary>
        /// Update the quantity of a product in the inventory
        /// </summary>
        /// <param name="quantityRequest">The new quantity information for the product</param>
        /// <param name="id">The product identifier</param>
        /// <returns>The updated product details</returns>
        /// <response code="200">Returns the updated product details</response>
        /// <response code="400">If the request validation failed</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have the required role</response>
        /// <response code="404">If the product was not found</response>
        [HttpPut("inventory/{id}/quantity")]
        [Authorize(Roles = "Manager, Inventory")]
        [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProductQuantity([FromBody] UpdateProductQuantityRequest quantityRequest, [FromRoute] int id)
        {
            var product = await _service.PatchProductQuantityAsync(id, quantityRequest.Quantity);

            var productResponse = new GetProductResponse(
                id: product.Id,
                name: product.Name,
                description: product.Description,
                quantity: product.Quantity,
                sellingPrice: product.Price,
                categories: product.Categories.Select((c) => c.Name).ToList()
            );

            return Ok(productResponse);
        }
    }
}
