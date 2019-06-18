namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusToMetaReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaReport", "ReportStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaReport", "ReportStatus");
        }
    }
}
