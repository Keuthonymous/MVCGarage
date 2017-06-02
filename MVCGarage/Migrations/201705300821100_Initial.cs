namespace MVCGarage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ParkingSpots",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    VehicleID = c.Int(),
                    Label = c.String(),
                    Fee = c.Double(nullable: false),
                    VehicleType = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Vehicles",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    VehicleType = c.Int(nullable: false),
                    Owner = c.String(),
                    RegistrationPlate = c.String(),
                    CheckInTime = c.DateTime(nullable: false),
                    ParkingSpotID = c.Int(),
                })
                .PrimaryKey(t => t.ID);

        }

        public override void Down()
        {
            DropTable("dbo.Vehicles");
            DropTable("dbo.ParkingSpots");
        }
    }
}
