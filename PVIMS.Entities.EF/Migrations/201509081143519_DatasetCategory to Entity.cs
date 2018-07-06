namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatasetCategorytoEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DatasetCategoryElement", "DatasetCategory_Id", "dbo.DatasetCategory");
            DropForeignKey("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id", "dbo.DatasetCategory");
            DropIndex("dbo.DatasetCategoryElement", new[] { "DatasetCategory_Id" });
            DropIndex("dbo.WorkPlanCareEventDatasetCategory", new[] { "DatasetCategory_Id" });
            DropPrimaryKey("dbo.DatasetCategory");
            AlterColumn("dbo.DatasetCategory", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DatasetCategoryElement", "DatasetCategory_Id", c => c.Int());
            AlterColumn("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id", c => c.Int());
            AddPrimaryKey("dbo.DatasetCategory", "Id");
            CreateIndex("dbo.DatasetCategoryElement", "DatasetCategory_Id");
            CreateIndex("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id");
            AddForeignKey("dbo.DatasetCategoryElement", "DatasetCategory_Id", "dbo.DatasetCategory", "Id");
            AddForeignKey("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id", "dbo.DatasetCategory", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id", "dbo.DatasetCategory");
            DropForeignKey("dbo.DatasetCategoryElement", "DatasetCategory_Id", "dbo.DatasetCategory");
            DropIndex("dbo.WorkPlanCareEventDatasetCategory", new[] { "DatasetCategory_Id" });
            DropIndex("dbo.DatasetCategoryElement", new[] { "DatasetCategory_Id" });
            DropPrimaryKey("dbo.DatasetCategory");
            AlterColumn("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id", c => c.Long());
            AlterColumn("dbo.DatasetCategoryElement", "DatasetCategory_Id", c => c.Long());
            AlterColumn("dbo.DatasetCategory", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DatasetCategory", "Id");
            CreateIndex("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id");
            CreateIndex("dbo.DatasetCategoryElement", "DatasetCategory_Id");
            AddForeignKey("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id", "dbo.DatasetCategory", "Id");
            AddForeignKey("dbo.DatasetCategoryElement", "DatasetCategory_Id", "dbo.DatasetCategory", "Id");
        }
    }
}
