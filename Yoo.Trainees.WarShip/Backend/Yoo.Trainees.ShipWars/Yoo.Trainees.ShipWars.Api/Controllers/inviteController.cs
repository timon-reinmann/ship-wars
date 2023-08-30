using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Yoo.Trainees.ShipWars.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class inviteController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            Guid id = Guid.NewGuid();
            string url = value + id.ToString();
            return Ok(new { response = url });
        }
    }
}
