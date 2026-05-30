using DesignParkingLot.Domain.Enums;

namespace DesignParkingLot.Domain.Entities
{
    public class Vehicle
    {
        
        public string NumberPlate { get; }
        public VehicleType Type { get; }
        public Vehicle(string numberPlate, VehicleType type)
        {
            NumberPlate = numberPlate;
            Type = type;
        }
    }
}
