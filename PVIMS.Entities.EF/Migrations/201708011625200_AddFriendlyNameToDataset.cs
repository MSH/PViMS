namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFriendlyNameToDataset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetCategory", "FriendlyName", c => c.String(maxLength: 50));
            AddColumn("dbo.DatasetCategoryElement", "FriendlyName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetCategoryElement", "FriendlyName");
            DropColumn("dbo.DatasetCategory", "FriendlyName");
        }
    }
}
