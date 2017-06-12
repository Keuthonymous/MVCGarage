using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCGarage.Models
{
    public class CheckIn
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Check in time")]
        [Required]
        public DateTime CheckInTime { get; set; }

        [Required]
        public bool Booked { get; set; }

        [Required]
        public bool Free { get; set; }

        // Navigation property - Allows the 1..1 relation to the "ParkingSpot" table
        [ForeignKey("ParkingSpot")]
        public int ParkingSpotID { get; set; }

        public virtual ParkingSpot ParkingSpot { get; set; }
        // --- //

        // Navigation property - Allows the 1..1 relation to the "Vehicle" table
        [ForeignKey("Vehicle")]
        public int VehicleID { get; set; }

        public virtual Vehicle Vehicle { get; set; }
        // --- //
    }
}