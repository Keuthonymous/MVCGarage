using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVCGarage.Models
{
    public class VehicleType
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Vehicle Type")]
        public string Type { get; set; }
    }
}