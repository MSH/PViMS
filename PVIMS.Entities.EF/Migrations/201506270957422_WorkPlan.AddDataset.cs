namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkPlanAddDataset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkPlan", "Dataset_Id", c => c.Int());
            AddColumn("dbo.Dataset", "EncounterTypeWorkPlan_Id", c => c.Int());
            CreateIndex("dbo.WorkPlan", "Dataset_Id");
            CreateIndex("dbo.Dataset", "EncounterTypeWorkPlan_Id");
            AddForeignKey("dbo.Dataset", "EncounterTypeWorkPlan_Id", "dbo.EncounterTypeWorkPlan", "Id");
            AddForeignKey("dbo.WorkPlan", "Dataset_Id", "dbo.Dataset", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkPlan", "Dataset_Id", "dbo.Dataset");
            DropForeignKey("dbo.Dataset", "EncounterTypeWorkPlan_Id", "dbo.EncounterTypeWorkPlan");
            DropIndex("dbo.Dataset", new[] { "EncounterTypeWorkPlan_Id" });
            DropIndex("dbo.WorkPlan", new[] { "Dataset_Id" });
            DropColumn("dbo.Dataset", "EncounterTypeWorkPlan_Id");
            DropColumn("dbo.WorkPlan", "Dataset_Id");
        }
    }
}
