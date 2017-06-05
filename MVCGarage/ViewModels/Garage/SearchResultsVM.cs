using MVCGarage.Models;
using MVCGarage.ViewModels.Shared;
using System.Collections.Generic;

namespace MVCGarage.ViewModels.Garage
{
    public class SearchResultsVM
    {
        public string SearchedValue { get; set; }
        public DisplayVehiclesVM FoundVehicles { get; set; }
        public IEnumerable<DetailsParkingSpotVM> FoundParkingSpots { get; set; }
    }
}