﻿using Microsoft.AspNetCore.Mvc;
using Yoo.Trainees.ShipWars.Api.Logic;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;
using Yoo.Trainees.ShipWars.DataBase.Migrations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Yoo.Trainees.ShipWars.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IGameLogic _gameLogic;
        private readonly IVerificationLogic _verificationLogic;
        private readonly IEmailSender _emailSender;
        private readonly IBotLogic _botLogic;

        public static List<Ship> Ships = new List<Ship>
        {
                new Ship { Length = 2, Name = "destroyer" },
                new Ship { Length = 4, Name = "warship" },
                new Ship { Length = 3, Name = "cruiser" },
                new Ship { Length = 1, Name = "submarine" }
        };

        public enum GameMode
        {
            hard,
            easy
        }

        public GameController(IGameLogic gameLogic, IEmailSender emailSender, IConfiguration configuration, IVerificationLogic verificationLogic, IBotLogic botLogic)
        {
            this._gameLogic = gameLogic;
            this._emailSender = emailSender;
            this._verificationLogic = verificationLogic;
            this._configuration = configuration;
            this._botLogic = botLogic;
        }

        // Ready checks in the DB if all ships are placed.
        [HttpGet("{gameId}/Ready")]
        public IActionResult Ready(Guid gameId)
        {
            if (_gameLogic.IsReady(gameId))
                return Ok();
            return BadRequest();
        }

        // Get all Messages if some one reloads the website
        [HttpGet("{gameId}/Message")]
        public IActionResult Message(Guid gameId)
        {
            var messages = _gameLogic.GetAllMessages(gameId);
            return Ok(messages);
        }

        // If Player reloads the Website it checks if he already has ships placed.
        [HttpGet("{gamePlayerId}/BoardState")]
        public IActionResult BoardState(Guid gamePlayerId)
        {
            var board = _gameLogic.GetCompleteShipPositionsForGamePlayer(gamePlayerId);
            if (board != null)
                return Ok(board);
            return BadRequest();
        }

        // It checks if gamePlayer is the NextPlayer --> Game.NextPlayer == gamePlayerId?
        [HttpGet("{gamePlayerId}/{gameId}/CheckReadyToShoot")]
        public IActionResult CheckReadyToShoot(Guid gameId, Guid gamePlayerId)
        {
            bool isBotLobby = _botLogic.IsBotLobby(gameId);
            if (_gameLogic.UpdateAndCheckNextPlayer(gameId, gamePlayerId) || isBotLobby)
                return Ok();
            return BadRequest();
        }

        [HttpPost("{gamePlayerId}/CheckBotHitShip")]
        public IActionResult CheckBotHitShip(Guid gamePlayerId, [FromBody] SaveShotsDto xy, Guid gameId)
        {
            try
            {
                var shipHit = _botLogic.CheckIfBotHit(xy, gamePlayerId);
                return Ok(new { hit = shipHit });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { bad = -1 });
            }
        }

        // It saves the Shot in the DB and returns if a ship was hit.
        [HttpPost("{gamePlayerId}/SaveShot")]
        public IActionResult SaveShot([FromBody] SaveShotsDto xy, Guid gameId, Guid gamePlayerId)
        {
            bool isBotLobby = _botLogic.IsBotLobby(gameId);
            try
            {
                if (isBotLobby)
                {
                    _gameLogic.VerifyAndSaveShot(xy, gamePlayerId);
                    var botGamePlayerId = _botLogic.GetBotGamePlayerId(gamePlayerId);
                    var shipHit = _gameLogic.CheckIfShipHit(xy, botGamePlayerId);
                    return Ok(new { hit = shipHit });

                }
                else
                {
                    _gameLogic.VerifyAndSaveShot(xy, gamePlayerId);
                    var shipHit = _gameLogic.CheckIfShipHit(xy, gamePlayerId);
                    _gameLogic.SaveShot(xy, gamePlayerId);
                    return Ok(new { hit = shipHit });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { bad = -1 });
            }
        }

        //
        [HttpGet("{gamePlayerId}/LoadHitShips")]
        public IActionResult LoadHitShips(Guid gamePlayerId)
        {
            var hitFields = _gameLogic.GetAllHitShipFields(gamePlayerId);
            return Ok(hitFields);
        }

        // To visualize the own Fields which got hit.
        [HttpGet("{gamePlayerId}/LoadShotsFromOpponent")]
        public IActionResult LoadShotsFromOpponent(Guid gamePlayerId)
        {
            var shots = _gameLogic.GetAllShotsOfOpponent(gamePlayerId);
            return Ok(shots);
        }

        // Get all yout Shots and visualize them.
        [HttpGet("{gamePlayerId}/LoadFiredShots")]
        public IActionResult LoadFiredShots(Guid gamePlayerId)
        {
            var shots = _gameLogic.ShotsAll(gamePlayerId);
            return Ok(shots);
        }

        // Check if player2 already has made a decision (Rock-Paper-Scissors).
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

        // Save your Rock-Paper-Scissors choice.
        [HttpPut("{gamePlayerId}/SaveSRP")]
        public IActionResult SaveSRP([FromBody] ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId)
        {
            _gameLogic.SaveChoiceIntoDB(scissorsRockPaperBet, gamePlayerId);
            return Ok();
        }

        // POST api/<GameController>.
        [HttpPost]
        public IActionResult Post(bool gameId, [FromBody] GameDto gameDto)
        {
            var game = _gameLogic.CreateGame(gameDto.Name, gameDto.Bot, gameDto.EasyGame);
            var isBotLobby = _botLogic.IsBotLobby(game.Id);
            var linkPlayer1 = CreateLink(game.Id, game.GamePlayers.First().Id, isBotLobby);
            var linkPlayer2 = CreateLink(game.Id, game.GamePlayers.ToArray()[1].Id, isBotLobby);

            var links = new
            {
                player1 = linkPlayer1,
                player2 = linkPlayer2,
            };
            return Ok(links);
        }

        // Post api/<Game>/5/SaveShips.
        [HttpPost("{id}/SaveShips")]
        public async Task<IActionResult> SaveShips( Guid id, [FromBody] SaveShipsDto ships)
        {
            var gameId = ships.GameId;
            if (id != gameId)
            {
                return BadRequest("Mismatch game ID");
            }
            bool isBotLobby = _botLogic.IsBotLobby(gameId);

            bool isValidRequest = _verificationLogic.VerifyEverything(ships.Ships);
            if (!isValidRequest)
            {
                return BadRequest();
            }

            if (!isBotLobby)
            {
                _gameLogic.CreateBoard(ships);
            }
            else
            {

                _botLogic.SaveShipPositionsInBotGame(ships);
            }
            return Ok();
        }

        // Send Email.
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

        // Count ALL shots for the counter and also give information for the nextplayer and game state (Ongoing, Lost, Won, Prep, Complete).
        [HttpGet("{gamePlayerId}/CountShots")]
        public IActionResult CountShots(Guid gamePlayerId)
        {
            ShotInfoDto countAndNextPlayer = _gameLogic.CountShotsInDB(gamePlayerId);

            var gameStateDB = _gameLogic.GetGameState(gamePlayerId);

            return Ok(new { shots = countAndNextPlayer.ShotCount, nextPlayer = countAndNextPlayer.IsNextPlayer, gameState = gameStateDB });
        }

        [HttpGet("{gamePlayerId}/GetUser")]
        public IActionResult GetUser(Guid gamePlayerId)
        {
            var user = _gameLogic.GetCurrentUser(gamePlayerId);
            return Ok(new { User = user });
        }

        [HttpGet("{gamePlayerId}/GetShotsFromBot")]
        public IActionResult GetBotShots(Guid gamePlayerId)
        {
            var botShotPositions = _botLogic.BotShotPosition(gamePlayerId);
            return Ok(new { BotShots = botShotPositions });
        }

        [HttpGet("{gamePlayerId}/LoadShotsFromBot")]
        public IActionResult LoadShotsFromBot(Guid gamePlayerId)
        {
            var game = _botLogic.GetGame(gamePlayerId);
            var botShots = _botLogic.GetAllBotShots(gamePlayerId, game.Id);
            return Ok(new { BotShots = botShots });
        }

        // Create link for invitation.
        private String CreateLink(Guid gameId, Guid gamePlayerId, bool isBotLobby)
        {

            return isBotLobby ? _configuration["Link:URL-PVE"] + gameId + "&gamePlayerId=" + gamePlayerId : _configuration["Link:URL-PVP"] + gameId + "&gamePlayerId=" + gamePlayerId;
        }

    }
}