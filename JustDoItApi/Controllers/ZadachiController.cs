using JustDoItApi.Models.Zadachi;
using Microsoft.AspNetCore.Mvc;

namespace JustDoItApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZadachiController : ControllerBase
    {

        [HttpGet()]
        public IActionResult Get()
        {
            var items = new List<ZadachaItemModel> { 
                new ZadachaItemModel() { Id = 1, Name = "ѕисати код" },
                new ZadachaItemModel() { Id = 2, Name = "ѕисати б≥льше коду!" },
                new ZadachaItemModel() { Id = 3, Name = "ѕисати ще б≥льше коду!!!" }
            };

            return Ok(items);
        }
    }
}
