using MVCGarage.Models;
using System;

namespace MVCGarage.ViewModels.Garage
{
    public class VehicleCheckedOutVM
    {
        public Vehicle Vehicle { get; set; }
        public ParkingSpot ParkingSpot { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int NbMinutes { get; set; }
        public double TotalAmount { get; set; }
    }
}