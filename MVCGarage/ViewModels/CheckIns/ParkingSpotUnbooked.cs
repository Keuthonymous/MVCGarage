﻿using MVCGarage.Models;
using System;

namespace MVCGarage.ViewModels.CheckIns
{
    public class ParkingSpotUnbooked
    {
        public Vehicle Vehicle { get; set; }
        public ParkingSpot ParkingSpot { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int NbMonths { get; set; }
        public double TotalAmount { get; set; }
    }
}