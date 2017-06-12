using MVCGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCGarage.DataAccess
{
    public class GarageContext : DbContext
    {
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<Owner> Owners { get; set; }

        public GarageContext()
            : base("DefaultConnection")
        {
        }
    }
}