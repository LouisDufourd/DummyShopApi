using DummyShopApi.API.DTO.Models;
using DummyShopApi.API.DTO.Request;
using DummyShopApi.API.DTO.Response;
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
    public class StockController : ControllerBase
    {
        private readonly IEcomService _service;
        public StockController(IEcomService ecomService) { 
            _service = ecomService;
        }

        [HttpGet("inventory")]

        [Authorize(Roles = "Manager, Inventory")]
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

        [HttpGet("inventory/{id}")]
        [Authorize(Roles = "Manager, Inventory")]
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

        [HttpPut("inventory/{id}/quantity")]
        [Authorize(Roles = "Manager, Inventory")]
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
