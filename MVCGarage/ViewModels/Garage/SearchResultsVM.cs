using MVCGarage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCGarage.ViewModels.Garage
{
    public class SearchResultsVM
    {
        public string SearchedValue { get; set; }
        public Vehicle FoundVehicle { get; set; }
        public ParkingSpot FoundParkingSpot { get; set; }
    }
}