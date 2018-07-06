namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyReportInstanceCurrentActivity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReportInstance", "CurrentActivity_Id", "dbo.ActivityInstance");
            DropIndex("dbo.ActivityInstance", new[] { "ReportInstance_Id" });
            DropIndex("dbo.ActivityInstance", new[] { "ReportInstance_Id1" });
            DropIndex("dbo.ReportInstance", new[] { "CurrentActivity_Id" });
            DropColumn("dbo.ActivityInstance", "ReportInstance_Id");
            RenameColumn(table: "dbo.ActivityInstance", name: "ReportInstance_Id1", newName: "ReportInstance_Id");
            AddColumn("dbo.ActivityInstance", "Current", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ActivityInstance", "ReportInstance_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ActivityInstance", "ReportInstance_Id");
            DropColumn("dbo.ReportInstance", "CurrentActivity_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReportInstance", "CurrentActivity_Id", c => c.Int());
            DropIndex("dbo.ActivityInstance", new[] { "ReportInstance_Id" });
            AlterColumn("dbo.ActivityInstance", "ReportInstance_Id", c => c.Int());
            DropColumn("dbo.ActivityInstance", "Current");
            RenameColumn(table: "dbo.ActivityInstance", name: "ReportInstance_Id", newName: "ReportInstance_Id1");
            AddColumn("dbo.ActivityInstance", "ReportInstance_Id", c => c.Int());
            CreateIndex("dbo.ReportInstance", "CurrentActivity_Id");
            CreateIndex("dbo.ActivityInstance", "ReportInstance_Id1");
            CreateIndex("dbo.ActivityInstance", "ReportInstance_Id");
            AddForeignKey("dbo.ReportInstance", "CurrentActivity_Id", "dbo.ActivityInstance", "Id");
        }
    }
}
