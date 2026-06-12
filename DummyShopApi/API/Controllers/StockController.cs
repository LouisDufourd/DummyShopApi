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
        private readonly IUOW _db;
        public StockController(IUOW db) { 
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var inventory = await _db.Inventory.GetAllAsync();

            return Ok(inventory);
        }
    }
}
