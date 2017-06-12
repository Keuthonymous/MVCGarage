namespace MVCGarage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BecauseItSaidTo : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Owners", "LicenseNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Owners", "LicenseNumber", c => c.String());
        }
    }
}
