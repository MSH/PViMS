namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWorkFlowEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QualifiedName = c.String(nullable: false, maxLength: 50),
                        ActivityType = c.Int(nullable: false),
                        WorkFlow_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkFlow", t => t.WorkFlow_Id)
                .Index(t => t.WorkFlow_Id);
            
            CreateTable(
                "dbo.WorkFlow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActivityExecutionStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActivityExecutionStatusEvent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventDateTime = c.DateTime(nullable: false),
                        Comments = c.String(),
                        ActivityInstance_Id = c.Int(nullable: false),
                        EventCreatedBy_Id = c.Int(),
                        ExecutionStatus_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityInstance", t => t.ActivityInstance_Id)
                .ForeignKey("dbo.User", t => t.EventCreatedBy_Id)
                .ForeignKey("dbo.ActivityExecutionStatus", t => t.ExecutionStatus_Id)
                .Index(t => t.ActivityInstance_Id)
                .Index(t => t.EventCreatedBy_Id)
                .Index(t => t.ExecutionStatus_Id);
            
            CreateTable(
                "dbo.ActivityInstance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QualifiedName = c.String(nullable: false, maxLength: 50),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        ReportInstance_Id = c.Int(nullable: false),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.ReportInstance", t => t.ReportInstance_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.ReportInstance_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.ReportInstance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReportInstanceGuid = c.Guid(nullable: false),
                        Finished = c.DateTime(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                        WorkFlow_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .ForeignKey("dbo.WorkFlow", t => t.WorkFlow_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.UpdatedBy_Id)
                .Index(t => t.WorkFlow_Id);
            
            DropColumn("dbo.DatasetInstance", "InstanceStatus");
            DropColumn("dbo.DatasetInstance", "ProcessStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DatasetInstance", "ProcessStatus", c => c.Int(nullable: false));
            AddColumn("dbo.DatasetInstance", "InstanceStatus", c => c.Int(nullable: false));
            DropForeignKey("dbo.ActivityExecutionStatusEvent", "ExecutionStatus_Id", "dbo.ActivityExecutionStatus");
            DropForeignKey("dbo.ActivityExecutionStatusEvent", "EventCreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.ActivityExecutionStatusEvent", "ActivityInstance_Id", "dbo.ActivityInstance");
            DropForeignKey("dbo.ActivityInstance", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.ActivityInstance", "ReportInstance_Id", "dbo.ReportInstance");
            DropForeignKey("dbo.ReportInstance", "WorkFlow_Id", "dbo.WorkFlow");
            DropForeignKey("dbo.ReportInstance", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.ReportInstance", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.ActivityInstance", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Activity", "WorkFlow_Id", "dbo.WorkFlow");
            DropIndex("dbo.ReportInstance", new[] { "WorkFlow_Id" });
            DropIndex("dbo.ReportInstance", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.ReportInstance", new[] { "CreatedBy_Id" });
            DropIndex("dbo.ActivityInstance", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.ActivityInstance", new[] { "ReportInstance_Id" });
            DropIndex("dbo.ActivityInstance", new[] { "CreatedBy_Id" });
            DropIndex("dbo.ActivityExecutionStatusEvent", new[] { "ExecutionStatus_Id" });
            DropIndex("dbo.ActivityExecutionStatusEvent", new[] { "EventCreatedBy_Id" });
            DropIndex("dbo.ActivityExecutionStatusEvent", new[] { "ActivityInstance_Id" });
            DropIndex("dbo.Activity", new[] { "WorkFlow_Id" });
            DropTable("dbo.ReportInstance");
            DropTable("dbo.ActivityInstance");
            DropTable("dbo.ActivityExecutionStatusEvent");
            DropTable("dbo.ActivityExecutionStatus");
            DropTable("dbo.WorkFlow");
            DropTable("dbo.Activity");
        }
    }
}
