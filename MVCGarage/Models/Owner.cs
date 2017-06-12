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
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string Fname { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string Lname { get; set; }
        
        [StringLength(1, ErrorMessage = "Please enter M for male and F for female")]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }
    }
}