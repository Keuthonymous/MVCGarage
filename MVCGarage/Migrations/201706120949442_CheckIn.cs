namespace MVCGarage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CheckIn : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CheckIns",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CheckInTime = c.DateTime(nullable: false),
                        Booked = c.Boolean(nullable: false),
                        Free = c.Boolean(nullable: false),
                        ParkingSpotID = c.Int(nullable: false),
                        VehicleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ParkingSpots", t => t.ParkingSpotID, cascadeDelete: true)
                .ForeignKey("dbo.Vehicles", t => t.VehicleID, cascadeDelete: true)
                .Index(t => t.ParkingSpotID)
                .Index(t => t.VehicleID);
            
            CreateTable(
                "dbo.VehicleTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CheckIns", "VehicleID", "dbo.Vehicles");
            DropForeignKey("dbo.CheckIns", "ParkingSpotID", "dbo.ParkingSpots");
            DropIndex("dbo.CheckIns", new[] { "VehicleID" });
            DropIndex("dbo.CheckIns", new[] { "ParkingSpotID" });
            DropTable("dbo.VehicleTypes");
            DropTable("dbo.CheckIns");
        }
    }
}
