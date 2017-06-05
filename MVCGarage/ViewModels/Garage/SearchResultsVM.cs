using MVCGarage.Models;
using MVCGarage.ViewModels.Shared;

namespace MVCGarage.ViewModels.Garage
{
    public class SearchResultsVM
    {
        public string SearchedValue { get; set; }
        public Vehicle FoundVehicle { get; set; }
        public DetailsParkingSpotVM FoundParkingSpot { get; set; }
    }
}