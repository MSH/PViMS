namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCurrentStatusToActivityInstance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityExecutionStatus", "Activity_Id", c => c.Int(nullable: false));
            AddColumn("dbo.ActivityInstance", "CurrentStatus_Id", c => c.Int());
            CreateIndex("dbo.ActivityExecutionStatus", "Activity_Id");
            CreateIndex("dbo.ActivityInstance", "CurrentStatus_Id");
            AddForeignKey("dbo.ActivityExecutionStatus", "Activity_Id", "dbo.Activity", "Id");
            AddForeignKey("dbo.ActivityInstance", "CurrentStatus_Id", "dbo.ActivityExecutionStatus", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivityInstance", "CurrentStatus_Id", "dbo.ActivityExecutionStatus");
            DropForeignKey("dbo.ActivityExecutionStatus", "Activity_Id", "dbo.Activity");
            DropIndex("dbo.ActivityInstance", new[] { "CurrentStatus_Id" });
            DropIndex("dbo.ActivityExecutionStatus", new[] { "Activity_Id" });
            DropColumn("dbo.ActivityInstance", "CurrentStatus_Id");
            DropColumn("dbo.ActivityExecutionStatus", "Activity_Id");
        }
    }
}
