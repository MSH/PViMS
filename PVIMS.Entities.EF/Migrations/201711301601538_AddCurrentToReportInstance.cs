namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCurrentToReportInstance : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActivityInstance", "ReportInstance_Id", "dbo.ReportInstance");
            DropIndex("dbo.ActivityInstance", new[] { "ReportInstance_Id" });
            AddColumn("dbo.ActivityInstance", "ReportInstance_Id1", c => c.Int(nullable: false));
            AddColumn("dbo.ReportInstance", "CurrentActivity_Id", c => c.Int(nullable: false));
            AddColumn("dbo.ReportInstance", "CurrentStatus_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.ActivityInstance", "ReportInstance_Id", c => c.Int());
            CreateIndex("dbo.ActivityInstance", "ReportInstance_Id");
            CreateIndex("dbo.ActivityInstance", "ReportInstance_Id1");
            CreateIndex("dbo.ReportInstance", "CurrentActivity_Id");
            CreateIndex("dbo.ReportInstance", "CurrentStatus_Id");
            AddForeignKey("dbo.ReportInstance", "CurrentActivity_Id", "dbo.ActivityInstance", "Id");
            AddForeignKey("dbo.ReportInstance", "CurrentStatus_Id", "dbo.ActivityExecutionStatus", "Id");
            AddForeignKey("dbo.ActivityInstance", "ReportInstance_Id1", "dbo.ReportInstance", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivityInstance", "ReportInstance_Id1", "dbo.ReportInstance");
            DropForeignKey("dbo.ReportInstance", "CurrentStatus_Id", "dbo.ActivityExecutionStatus");
            DropForeignKey("dbo.ReportInstance", "CurrentActivity_Id", "dbo.ActivityInstance");
            DropIndex("dbo.ReportInstance", new[] { "CurrentStatus_Id" });
            DropIndex("dbo.ReportInstance", new[] { "CurrentActivity_Id" });
            DropIndex("dbo.ActivityInstance", new[] { "ReportInstance_Id1" });
            DropIndex("dbo.ActivityInstance", new[] { "ReportInstance_Id" });
            AlterColumn("dbo.ActivityInstance", "ReportInstance_Id", c => c.Int(nullable: false));
            DropColumn("dbo.ReportInstance", "CurrentStatus_Id");
            DropColumn("dbo.ReportInstance", "CurrentActivity_Id");
            DropColumn("dbo.ActivityInstance", "ReportInstance_Id1");
            CreateIndex("dbo.ActivityInstance", "ReportInstance_Id");
            AddForeignKey("dbo.ActivityInstance", "ReportInstance_Id", "dbo.ReportInstance", "Id");
        }
    }
}
