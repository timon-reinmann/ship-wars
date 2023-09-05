using Microsoft.AspNetCore.Mvc;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Yoo.Trainees.ShipWars.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public GameController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        // GET: api/<GameController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            applicationDbContext.Games.Add(new Game());
            applicationDbContext.SaveChanges();

            return new string[] { "value1", "value2" };
        }

        // GET api/<Game>/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            return "value";
        }

        // POST api/<GameController>
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            Guid id = Guid.NewGuid();
            string url = value + id.ToString();
            return Ok(new { response = url });
        }

        // PUT api/<GameController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GameController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        { 
        }
    }
}
