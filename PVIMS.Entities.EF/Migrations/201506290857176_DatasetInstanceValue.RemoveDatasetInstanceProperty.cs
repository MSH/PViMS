namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatasetInstanceValueRemoveDatasetInstanceProperty : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DatasetInstanceValue", new[] { "DatasetInstance_Id" });
            AlterColumn("dbo.DatasetInstanceValue", "DatasetInstance_Id", c => c.Int());
            CreateIndex("dbo.DatasetInstanceValue", "DatasetInstance_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DatasetInstanceValue", new[] { "DatasetInstance_Id" });
            AlterColumn("dbo.DatasetInstanceValue", "DatasetInstance_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.DatasetInstanceValue", "DatasetInstance_Id");
        }
    }
}
