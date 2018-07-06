namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatasetInstanceAndRelated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DatasetInstance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContextID = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        Dataset_Id = c.Int(nullable: false),
                        EncounterTypeWorkPlan_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.Dataset", t => t.Dataset_Id)
                .ForeignKey("dbo.EncounterTypeWorkPlan", t => t.EncounterTypeWorkPlan_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.Dataset_Id)
                .Index(t => t.EncounterTypeWorkPlan_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.DatasetInstanceValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InstanceValue = c.String(nullable: false),
                        DatasetElement_Id = c.Int(nullable: false),
                        DatasetInstance_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetElement", t => t.DatasetElement_Id)
                .ForeignKey("dbo.DatasetInstance", t => t.DatasetInstance_Id)
                .Index(t => t.DatasetElement_Id)
                .Index(t => t.DatasetInstance_Id);
            
            CreateTable(
                "dbo.DatasetInstanceSubValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContextValue = c.Guid(nullable: false),
                        InstanceValue = c.String(nullable: false),
                        DatasetElementSub_Id = c.Int(nullable: false),
                        DatasetInstanceValue_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetElementSub", t => t.DatasetElementSub_Id)
                .ForeignKey("dbo.DatasetInstanceValue", t => t.DatasetInstanceValue_Id)
                .Index(t => t.DatasetElementSub_Id)
                .Index(t => t.DatasetInstanceValue_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DatasetInstance", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.DatasetInstance", "EncounterTypeWorkPlan_Id", "dbo.EncounterTypeWorkPlan");
            DropForeignKey("dbo.DatasetInstanceSubValue", "DatasetInstanceValue_Id", "dbo.DatasetInstanceValue");
            DropForeignKey("dbo.DatasetInstanceSubValue", "DatasetElementSub_Id", "dbo.DatasetElementSub");
            DropForeignKey("dbo.DatasetInstanceValue", "DatasetInstance_Id", "dbo.DatasetInstance");
            DropForeignKey("dbo.DatasetInstanceValue", "DatasetElement_Id", "dbo.DatasetElement");
            DropForeignKey("dbo.DatasetInstance", "Dataset_Id", "dbo.Dataset");
            DropForeignKey("dbo.DatasetInstance", "CreatedBy_Id", "dbo.User");
            DropIndex("dbo.DatasetInstanceSubValue", new[] { "DatasetInstanceValue_Id" });
            DropIndex("dbo.DatasetInstanceSubValue", new[] { "DatasetElementSub_Id" });
            DropIndex("dbo.DatasetInstanceValue", new[] { "DatasetInstance_Id" });
            DropIndex("dbo.DatasetInstanceValue", new[] { "DatasetElement_Id" });
            DropIndex("dbo.DatasetInstance", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.DatasetInstance", new[] { "EncounterTypeWorkPlan_Id" });
            DropIndex("dbo.DatasetInstance", new[] { "Dataset_Id" });
            DropIndex("dbo.DatasetInstance", new[] { "CreatedBy_Id" });
            DropTable("dbo.DatasetInstanceSubValue");
            DropTable("dbo.DatasetInstanceValue");
            DropTable("dbo.DatasetInstance");
        }
    }
}
