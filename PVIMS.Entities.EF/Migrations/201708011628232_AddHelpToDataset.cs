namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHelpToDataset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetCategory", "Help", c => c.String(maxLength: 350));
            AddColumn("dbo.DatasetCategoryElement", "Help", c => c.String(maxLength: 350));
            AlterColumn("dbo.DatasetCategoryElement", "FriendlyName", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DatasetCategoryElement", "FriendlyName", c => c.String(maxLength: 50));
            DropColumn("dbo.DatasetCategoryElement", "Help");
            DropColumn("dbo.DatasetCategory", "Help");
        }
    }
}
