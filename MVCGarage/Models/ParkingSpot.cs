using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCGarage.Models
{
    public class ParkingSpot
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Identifiant")]
        public string Label { get; set; }

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

        [Display(Name = "Monthly fee")]
        public double MonthlyFee()
        {
            return Math.Round(70 * 30 * 24 * 60 * GetFee() / 100, 2, MidpointRounding.AwayFromZero);
        }

        // Navigation property - Allows the 1..1 relation to the "VehicleType" table
        [ForeignKey("VehicleType")]
        public int VehicleTypeID { get; set; }

        public virtual VehicleType VehicleType { get; set; }
        // --- //

        // Navigation property - Allows the 1..* relation to the "CheckIn" table
        public virtual ICollection<CheckIn> CheckIns { get; set; }
        // --- //
    }
}