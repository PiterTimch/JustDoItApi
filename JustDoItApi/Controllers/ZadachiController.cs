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
                new ZadachaItemModel() { Id = 1, Name = "ѕисати код", Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/18/Dog_Breeds.jpg/1024px-Dog_Breeds.jpg" },
                new ZadachaItemModel() { Id = 2, Name = "ѕисати б≥льше коду!", Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/18/Dog_Breeds.jpg/1024px-Dog_Breeds.jpg" },
                new ZadachaItemModel() { Id = 3, Name = "ѕисати ще б≥льше коду!!!", Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/18/Dog_Breeds.jpg/1024px-Dog_Breeds.jpg" }
            };

            return Ok(items);
        }
    }
}
