using MVCGarage.Models;

namespace MVCGarage.ViewModels.CheckIns
{
    public class VehicleCheckedInVM
    {
        public Vehicle Vehicle { get; set; }
        public ParkingSpot ParkingSpot { get; set; }
    }
}