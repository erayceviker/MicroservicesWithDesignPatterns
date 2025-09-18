using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stock.Api.Models;

namespace Stock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController(AppDbContext appDbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetStock()
        {
            return Ok(await appDbContext.Stocks.ToListAsync());
        }
    }
}
