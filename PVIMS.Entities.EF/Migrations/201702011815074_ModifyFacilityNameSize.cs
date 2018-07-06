namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyFacilityNameSize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Facility", "FacilityName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Facility", "FacilityName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
