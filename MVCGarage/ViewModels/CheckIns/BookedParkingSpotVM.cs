using MVCGarage.Models;

namespace MVCGarage.ViewModels.CheckIns
{
    public class BookedParkingSpotVM
    {
        public Vehicle Vehicle { get; set; }
        public ParkingSpot ParkingSpot { get; set; }
    }
}