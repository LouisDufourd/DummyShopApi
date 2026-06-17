using DummyShopApi.BLL.Interfaces;
using DummyShopApi.DAL;
using DummyShopApi.DAL.DAO.Postgrsql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DummyShopApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IEcomService _ecomService;
        public StockController(IEcomService ecomService) { 
            _ecomService = ecomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory([FromQuery] int page = 1)
        {
            var inventory = await _ecomService.GetProductsAsync(page);

            return Ok(inventory);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await _ecomService.GetProductDetail(id);
            return Ok(product);
        }
    }
}
