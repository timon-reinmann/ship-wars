﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly IGameLogic _gameLogic;
        private readonly IEmailSender _emailSender;
        private Game _Game;
        public static List<Ship> Ships = new List<Ship>
        {
                new Ship { Length = 2, Name = "destroyer" },
                new Ship { Length = 4, Name = "warship" },
                new Ship { Length = 3, Name = "cruiser" },
                new Ship { Length = 1, Name = "submarine" }
        };
        // ToDo muss nich mit Marcel angeschaut werden
        private readonly IVerificationLogic _verificationLogic;

        public GameController(IGameLogic gameLogic, IEmailSender emailSender, IVerificationLogic verificationLogic)
        {
            this._gameLogic = gameLogic;
            this._emailSender = emailSender;
            this._verificationLogic = verificationLogic;
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
            if (_gameLogic.IsReady(id))
                return Ok();
            return BadRequest();
        }

        //
        [HttpGet("{id}/BoardState")]
        public IActionResult BoardState(Guid id)
        {
            var board = _gameLogic.GetCompleteShipPositionsForGamePlayer(id);
            if (board != null)
                return Ok(board);
            return BadRequest();
        }

        //
        [HttpGet("{gamePlayerId}/{gameId}/CheckReadyToShoot")]
        public IActionResult CheckReadyToShoot(Guid gameId, Guid gamePlayerId)
        {
            if (_gameLogic.UpdateAndCheckNextPlayer(gameId, gamePlayerId))
                return Ok();
            return BadRequest();
        }

        //
        [HttpPost("{gamePlayerId}/SaveShot")]
        public IActionResult SaveShot([FromBody] SaveShotsDto xy, Guid gamePlayerId)
        {
            try
            {
                _gameLogic.VerifyAndExecuteShotOrThrow(xy, gamePlayerId);
                var shipHit = _gameLogic.CheckIfShipHit(xy, gamePlayerId);
                _gameLogic.SaveShot(xy, gamePlayerId);
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
            var shots = _gameLogic.GetAllShotsOfOpponent(gamePlayerId);
            return Ok(shots);
        }

        //
        [HttpGet("{gamePlayerId}/LoadFiredShots")]
        public IActionResult LoadFiredShots(Guid gamePlayerId)
        {
            var shots = _gameLogic.ShotsAll(gamePlayerId);
            return Ok(shots);
        }

        //
        [HttpGet("{gamePlayerId}/CheckIfSRPIsSet")]
        public IActionResult CheckIfSRPIsSet(Guid gamePlayerId)
        {
            RockPaperScissorsState status = _gameLogic.GetResultOfTheSRP(gamePlayerId);
            if (status == RockPaperScissorsState.Won)
                return Ok(new { status = status });
            if (status == RockPaperScissorsState.Lost)
                return Ok(new { status = status });
            return BadRequest(new { status = status });
        }

        //
        [HttpPut("{gamePlayerId}/SaveSRP")]
        public IActionResult SaveSRP([FromBody] ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId)
        {
            _gameLogic.SaveChoiceIntoDB(scissorsRockPaperBet, gamePlayerId);
            return Ok();
        }

        // POST api/<GameController>
        [HttpPost]
        public IActionResult Post([FromBody] string name)
        {
            this._Game = _gameLogic.CreateGame(name);
            var linkPlayer1 = CreateLink(this._Game.Id, this._Game.GamePlayers.First().Id);
            var linkPlayer2 = CreateLink(this._Game.Id, this._Game.GamePlayers.ToArray()[1].Id);

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

            bool isValidRequest = _verificationLogic.VerifyEverything(Ships.Ships);
            if (!isValidRequest)
            {
                return BadRequest();
            }

            _gameLogic.CreateBoard(Ships);
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
            ShotInfoDto countAndNextPlayer = _gameLogic.CountShotsInDB(gamePlayerId);

            var gameStateDB = _gameLogic.GetGameState(gamePlayerId);

            return Ok(new { shots = countAndNextPlayer.ShotCount, nextPlayer = countAndNextPlayer.IsNextPlayer, gameState = gameStateDB });
        }

        private static String CreateLink(Guid gameId, Guid gamePlayerId)
        {
            var serverURL = "http://127.0.0.1:5500/Frontend/html/game-pvp.html?gameId=";
            return serverURL + gameId + "&gamePlayerId=" + gamePlayerId;
        }
    }
}