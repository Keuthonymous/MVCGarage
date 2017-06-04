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

        [Display(Name = "Check in time")]
        public DateTime? CheckInTime { get; set; }

        [Display(Name = "Fee")]
        public double? Fee { get; set; }

        public double GetFee()
        {
            if (Fee == null)
                return 0;
            else
                return (double)(Fee);
        }

        public string DisplayFee()
        {
            return string.Format("{0:N2}/min.", GetFee());
        }

        public string DisplayMonthlyFee()
        {
            return string.Format("{0:N2}/month", MonthlyFee());
        }

        [Display(Name = "Reserved vehicle type")]
        public ETypeVehicle VehicleType { get; set; }

        [Display(Name = "Monthly fee")]
        public double MonthlyFee()
        {
            return Math.Round(70 * 30 * 24 * 60 * GetFee() / 100, 2, MidpointRounding.AwayFromZero);
        }
    }
}