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
        private Game Game;
        private static List<Ship> Ships = new List<Ship>
        {
                new Ship { Length = 2, Name = "destroyer" },
                new Ship { Length = 4, Name = "warship" },
                new Ship { Length = 3, Name = "cruiser" },
                new Ship { Length = 1, Name = "submarine" }
        };
        // ToDo muss nich mit Marcel angeschaut werden
        private readonly VerificationLogic verificationLogic = new VerificationLogic(Ships);

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
        
        [HttpGet("{id}/Ready")]
        public IActionResult Ready(Guid id)
        {
            if (gameLogic.IsReady(id))
                return Ok();
            return BadRequest();
        }

        //
        [HttpGet("{id}/BoardState")]
        public IActionResult BoardState(Guid id)
        {
            var board = gameLogic.IsComplete(id);
            if (board != null)
                return Ok(board);
            return BadRequest();
        }

        //
        [HttpGet("{gamePlayerId}/{gameId}/CheckReadyToShoot")]
        public IActionResult CheckReadyToShoot(Guid gameId, Guid gamePlayerId)
        {
            if (gameLogic.CheckShots(gameId, gamePlayerId))
                return Ok();
            return BadRequest();
        }

        //
        [HttpPost("{gamePlayerId}/SaveShotInDB")]
        public IActionResult SaveShotInDB([FromBody] SaveShotsDto xy, Guid gamePlayerId)
        {
            try
            {
                gameLogic.VerifyAndExecuteShotOrThrow(xy, gamePlayerId);
                var shipHit = gameLogic.CheckIfShipHit(xy, gamePlayerId, Ships);
                gameLogic.SaveShot(xy, gamePlayerId);
                return Ok(new { hit = shipHit });
            } 
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { bad = -1 });
            }
        }

        //
        [HttpGet("{gamePlayerId}/LoadShotsFromOpponent")]
        public IActionResult LoadShotsFromOpponent(Guid gamePlayerId)
        {
            var shots = gameLogic.ShotsAllOpponent(gamePlayerId);
            return Ok(shots);
        }

        //
        [HttpGet("{gamePlayerId}/LoadFiredShots")]
        public IActionResult LoadFiredShots(Guid gamePlayerId)
        {
            var shots = gameLogic.ShotsAll(gamePlayerId);
            return Ok(shots);
        }

        //
        [HttpGet("{gamePlayerId}/CheckIfSRPIsSet")]
        public IActionResult CheckIfSRPIsSet(Guid gamePlayerId)
        {
            SRPState status = gameLogic.GetResultOfTheSRP(gamePlayerId);
            if (status == SRPState.WON)
                return Ok(new { status = status });
            if (status == SRPState.LOST)
                return Ok(new { status = status });
            return BadRequest(new { status = status });
        }

        //
        [HttpPut("{gamePlayerId}/SaveSRP")]
        public IActionResult SaveSRP([FromBody] ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId)
        {
            gameLogic.SaveChoiceIntoDB(scissorsRockPaperBet, gamePlayerId);
            return Ok(new { ok = true});
        }

        // POST api/<GameController>
        [HttpPost]
        public IActionResult Post([FromBody] string name)
        {
            this.Game = gameLogic.CreateGame(name);
            var linkPlayer1 = CreateLink(this.Game.Id, this.Game.GamePlayers.First().Id, Game.GamePlayers.First().PlayerId);
            var linkPlayer2 = CreateLink(this.Game.Id, this.Game.GamePlayers.ToArray()[1].Id, this.Game.GamePlayers.ToArray()[1].PlayerId);

            var links = new 
            {
                player1 = linkPlayer1,
                player2 = linkPlayer2,
            };
            return Ok(links);
        }

        // Post api/<Game>/5/SaveShips
        [HttpPost("{id}/SaveShips")]
        public async Task<IActionResult> Post(Guid id, [FromBody] SaveShipsDto Ships)
        {
            if (id != Ships.GameId)
            {
                return BadRequest("Mismatched game ID");
            }

            bool isValidRequest = verificationLogic.VerifyEverything(Ships.Ships);
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

        //
        [HttpGet("{gamePlayerId}/CountShots")]
        public IActionResult CountShots(Guid gamePlayerId)
        {
            int[] countAndNextPlayer = gameLogic.CountShotsInDB(gamePlayerId);

            GameState gameStateDB = gameLogic.CheckGameState(gamePlayerId);

            return Ok(new { shots = countAndNextPlayer[0], nextPlayer = countAndNextPlayer[1], gameState = gameStateDB });
        }

        private static String CreateLink(Guid gameId, Guid gamePlayerId, Guid playerId)
        {
            return "http://127.0.0.1:5500/Frontend/html/game-pvp.html?gameId=" + gameId + "&gamePlayerId=" + gamePlayerId + "&playerId=" + playerId;
        }
    }
}