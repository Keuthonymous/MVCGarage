using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCGarage.Models
{
    public class Owner
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Fname { get; set; }
        [Required]
        public string Lname { get; set; }
        
        [StringLength(1, ErrorMessage = "Please enter M for male and F for female")]
        public string Gender { get; set; }
        public string LicenseNumber { get; set; }
    }
}