namespace MVCGarage.Migrations
{
    using MVCGarage.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MVCGarage.DataAccess.GarageContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MVCGarage.DataAccess.GarageContext context)
        {
            context.ParkingSpots.AddOrUpdate(p => p.ID,
                new ParkingSpot { Label = "101", VehicleType = ETypeVehicle.car, Fee = 0.20 },
                new ParkingSpot { Label = "102", VehicleType = ETypeVehicle.car, Fee = 0.20 },
                new ParkingSpot { Label = "103", VehicleType = ETypeVehicle.motorcycle, Fee = 0.50 },
                new ParkingSpot { Label = "104", VehicleType = ETypeVehicle.motorcycle, Fee = 0.50 },
                new ParkingSpot { Label = "201", VehicleType = ETypeVehicle.truck, Fee = 0.80 },
                new ParkingSpot { Label = "202", VehicleType = ETypeVehicle.bus, Fee = 1.00 });

            context.Vehicles.AddOrUpdate(v => v.ID,
                new Vehicle { RegistrationPlate = "ABC123", VehicleType = ETypeVehicle.car, Owner = "Owner" },
                new Vehicle { RegistrationPlate = "ABC124", VehicleType = ETypeVehicle.car, Owner = "Owner Two" },
                new Vehicle { RegistrationPlate = "ABC125", VehicleType = ETypeVehicle.motorcycle, Owner = "Owner Three" },
                new Vehicle { RegistrationPlate = "ABC126", VehicleType = ETypeVehicle.motorcycle, Owner = "Owner Four" },
                new Vehicle { RegistrationPlate = "ABC127", VehicleType = ETypeVehicle.car, Owner = "Owner Five" },
                new Vehicle { RegistrationPlate = "ABC128", VehicleType = ETypeVehicle.truck, Owner = "Owner Six" },
                new Vehicle { RegistrationPlate = "ABC129", VehicleType = ETypeVehicle.bus, Owner = "Owner Seven" });

            context.Owners.AddOrUpdate(o => o.ID,
                new Owner { Fname = "Mike", Lname = "Daughtrey", Gender = "M", LicenseNumber = "ABC-123-DEF" },
                new Owner { Fname = "Wilhelm", Lname = "Hanssonithink", Gender = "M", LicenseNumber = "ABC-124-DEF" },
                new Owner { Fname = "Liam", Lname = "Nottoosure", Gender = "M", LicenseNumber = "ABC-125-DEF" });

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
