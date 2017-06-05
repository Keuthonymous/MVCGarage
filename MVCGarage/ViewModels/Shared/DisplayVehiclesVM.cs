using MVCGarage.Models;
using System.Collections.Generic;

namespace MVCGarage.ViewModels.Shared
{
    public class DisplayVehiclesVM
    {
        public string ViewName { get; set; }
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public Dictionary<int, ParkingSpot> ParkingSpotsVehicles { get; set; }
        public Dictionary<int, ParkingSpot> BookedParkingSpots { get; set; }
    }
}