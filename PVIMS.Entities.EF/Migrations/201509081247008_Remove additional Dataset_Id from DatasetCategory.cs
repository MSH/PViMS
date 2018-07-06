namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveadditionalDataset_IdfromDatasetCategory : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DatasetCategory", new[] { "Dataset_Id1" });
            DropColumn("dbo.DatasetCategory", "Dataset_Id");
            RenameColumn(table: "dbo.DatasetCategory", name: "Dataset_Id1", newName: "Dataset_Id");
            AlterColumn("dbo.DatasetCategory", "Dataset_Id", c => c.Int());
            CreateIndex("dbo.DatasetCategory", "Dataset_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DatasetCategory", new[] { "Dataset_Id" });
            AlterColumn("dbo.DatasetCategory", "Dataset_Id", c => c.Long(nullable: false));
            RenameColumn(table: "dbo.DatasetCategory", name: "Dataset_Id", newName: "Dataset_Id1");
            AddColumn("dbo.DatasetCategory", "Dataset_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.DatasetCategory", "Dataset_Id1");
        }
    }
}
