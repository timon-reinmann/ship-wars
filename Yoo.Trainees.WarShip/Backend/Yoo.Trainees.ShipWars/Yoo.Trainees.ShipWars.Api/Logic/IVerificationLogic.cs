namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IVerificationLogic
    {
        bool VerifyEverything(SaveShipDto[] shipDtos);
        bool VerifyShipLocations(SaveShipDto[] shipDtos);
        bool TestVerifyeToManyShipsFromSameType(SaveShipDto[] shipDtos);
        bool VerifyShot(List<SaveShotsDto> shotsDto, SaveShotsDto shot);
        SaveShipDto VerifyShipHit(List<SaveShipDto> shipsDB, SaveShotsDto shot);
        public bool VerifyShipPositionBot(SaveShipDto[] shipDtos);
    }
}
