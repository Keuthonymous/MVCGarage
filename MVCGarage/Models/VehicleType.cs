using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotation;

namespace MVCGarage.Models
{
    public class VehicleType
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        internal void CheckOut()
        {
            throw new NotImplementedException();
        }
    }
}