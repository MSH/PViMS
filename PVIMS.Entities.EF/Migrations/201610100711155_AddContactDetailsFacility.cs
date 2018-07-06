namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContactDetailsFacility : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facility", "TelNumber", c => c.String(maxLength: 30));
            AddColumn("dbo.Facility", "MobileNumber", c => c.String(maxLength: 30));
            AddColumn("dbo.Facility", "FaxNumber", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facility", "FaxNumber");
            DropColumn("dbo.Facility", "MobileNumber");
            DropColumn("dbo.Facility", "TelNumber");
        }
    }
}
