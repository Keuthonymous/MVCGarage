﻿using MVCGarage.Models;
using System.Collections.Generic;

namespace MVCGarage.ViewModels.Garage
{
    public class SelectAParkingSpotVM
    {
        public int VehicleID { get; set; }
        public Vehicle SelectedVehicle { get; set; }

        public IEnumerable<ParkingSpot> ParkingSpots { get; set; }
        public int ParkingSpotID { get; set; }
        public bool CheckIn { get; set; }

        public string FollowingActionName { get; set; }
        public string FollowingControllerName { get; set; }
    }
}