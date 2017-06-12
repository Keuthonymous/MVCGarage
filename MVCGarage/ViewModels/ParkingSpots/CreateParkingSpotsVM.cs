using MVCGarage.Models;
using System.Collections.Generic;

namespace MVCGarage.ViewModels.ParkingSpots
{
    public class CreateParkingSpotsVM
    {
        public int VehicleTypeId { get; set; }
        public ParkingSpot ParkingSpot { get; set; }
        public Dictionary<int, double> DefaultFees { get; set; }
        public string ErrorMessage { get; set; }
    }
}