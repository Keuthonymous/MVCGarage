﻿using MVCGarage.Models;

namespace MVCGarage.ViewModels.ParkingSpots
{
    public class DetailsParkingSpotVM
    {
        public string Availability { get; set; }
        public ParkingSpot ParkingSpot { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}