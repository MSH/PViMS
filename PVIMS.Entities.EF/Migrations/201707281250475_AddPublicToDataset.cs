namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPublicToDataset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetCategory", "Public", c => c.Boolean(nullable: false));
            AddColumn("dbo.DatasetCategoryElement", "Public", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetCategoryElement", "Public");
            DropColumn("dbo.DatasetCategory", "Public");
        }
    }
}
