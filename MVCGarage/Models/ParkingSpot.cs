using System;
using System.ComponentModel.DataAnnotations;

namespace MVCGarage.Models
{
    public class ParkingSpot
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Vehicle ID")]
        public int? VehicleID { get; set; }

        [Display(Name = "Identifiant")]
        public string Label { get; set; }

        [Display(Name = "Fee")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double Fee { get; set; }

        [Display(Name = "Reserved vehicle type")]
        public ETypeVehicle VehicleType { get; set; }

        [Display(Name = "Monthly fee")]
        public double MonthlyFee()
        {
            return Math.Round( 70 * 30 * 24 * 60 * Fee / 100, 2, MidpointRounding.AwayFromZero);
        }

        public string Available()
        {
            if (VehicleID == null)
                return "Yes";
            else
                return "No";
        }
    }
}