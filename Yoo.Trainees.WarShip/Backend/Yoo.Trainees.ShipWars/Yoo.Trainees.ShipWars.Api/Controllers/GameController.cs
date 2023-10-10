using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Yoo.Trainees.ShipWars.Api.Logic;
using Yoo.Trainees.ShipWars.DataBase.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Yoo.Trainees.ShipWars.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameLogic gameLogic;
        private readonly IEmailSender _emailSender;
        // ToDo muss nich mit Marcel angeschaut werden
        private readonly VerificationLogic verificationLogic = new VerificationLogic(new List<Ship>
            {
                new Ship { Length = 2, Name = "Destroyer" },
                new Ship { Length = 4, Name = "Warship" },
                new Ship { Length = 3, Name = "Cruiser" },
                new Ship { Length = 1, Name = "Submarine" }
            });

        public GameController(IGameLogic gameLogic, IEmailSender emailSender)
        {
            this.gameLogic = gameLogic;
            this._emailSender = emailSender;
        }

        // GET: api/Game
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //[Route("FinishedGames")]

        // GET api/<Game>/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            return "value";
        }

        // POST api/<GameController>
        [HttpPost]
        public String Post([FromBody] string name)
        {
            var createdGame = gameLogic.CreateGame(name);
            return createdGame.Id.ToString();
        }

        // Post api/<Game>/5/SaveShips
        [HttpPost("{id}/SaveShips")]
        public async Task<IActionResult> Post([FromBody] SaveShipsDto Ships)
        {
            bool isValidRequest = verificationLogic.verifyEvrything(Ships.Ships);
            if (!isValidRequest)
            {
                return BadRequest();
            }
            gameLogic.CreateBoard(Ships);
            return Ok();
        }

        [Route("Email")]
        [HttpPost]
        public async Task<IActionResult> NotifyGameAsync([FromBody] EmailDto body)
        {
            await _emailSender.SendEmailAsync(
                body.Email,
                "Neues Spiel erstellt",
                $"Du wurdest zu einem Spiel namens {body.LobbyName} eingeladen! Link zum Spiel: {body.Link}"
            );
            return Ok();
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
