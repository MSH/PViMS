namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdentifierToReportInstance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportInstance", "Identifier", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportInstance", "Identifier");
        }
    }
}
