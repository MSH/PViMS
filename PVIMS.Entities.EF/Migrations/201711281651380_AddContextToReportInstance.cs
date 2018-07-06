namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContextToReportInstance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportInstance", "ContextGuid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportInstance", "ContextGuid");
        }
    }
}
