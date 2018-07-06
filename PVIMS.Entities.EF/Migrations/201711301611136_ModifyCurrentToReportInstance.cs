namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCurrentToReportInstance : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReportInstance", "CurrentStatus_Id", "dbo.ActivityExecutionStatus");
            DropIndex("dbo.ReportInstance", new[] { "CurrentActivity_Id" });
            DropIndex("dbo.ReportInstance", new[] { "CurrentStatus_Id" });
            AlterColumn("dbo.ReportInstance", "CurrentActivity_Id", c => c.Int());
            CreateIndex("dbo.ReportInstance", "CurrentActivity_Id");
            DropColumn("dbo.ReportInstance", "CurrentStatus_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReportInstance", "CurrentStatus_Id", c => c.Int(nullable: false));
            DropIndex("dbo.ReportInstance", new[] { "CurrentActivity_Id" });
            AlterColumn("dbo.ReportInstance", "CurrentActivity_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ReportInstance", "CurrentStatus_Id");
            CreateIndex("dbo.ReportInstance", "CurrentActivity_Id");
            AddForeignKey("dbo.ReportInstance", "CurrentStatus_Id", "dbo.ActivityExecutionStatus", "Id");
        }
    }
}
