using DesignParkingLot.Domain.Enums;

namespace DesignParkingLot.Domain.Entities
{
    public class ParkingSpot
    {
        public string SpotId { get;}
        public ParkingSpotType Type { get; }
        public bool IsOccupied {get; set;}

        public ParkingSpot(string _spotId, ParkingSpotType _type)
        {
            SpotId = _spotId;
            Type = _type;
            IsOccupied = false;
        }

        public bool isSpotFree()
        {
            return !IsOccupied;
        }

        public bool allotSpot(Vehicle vehicle)
        {
            if(isSpotFree() && Type.ToString() == vehicle.Type.ToString())
            {
                IsOccupied = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void releaseSpot()
        {
            IsOccupied = false;
        }
    }
}