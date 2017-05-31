﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MVCGarage.Models
{
    public class Vehicle
    {
        [Key]
        [Display(Name = "Vehicle ID")]
        public int ID { get; set; }

        [Display(Name = "Vehicle type")]
        public ETypeVehicle VehicleType { get; set; }

        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [Display(Name = "Regitration plate")]
        public string RegistrationPlate { get; set; }

        [Display(Name = "Check in time")]
        public DateTime? CheckInTime { get; set; }

        [Display(Name = "Parking Spot ID")]
        public int? ParkingSpotID { get; set; }
    }
}