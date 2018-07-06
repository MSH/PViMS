namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Linkchronictoelement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetCategoryElement", "Acute", c => c.Boolean(nullable: false));
            AddColumn("dbo.DatasetCategoryElement", "Chronic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Condition", "Chronic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Condition", "Chronic");
            DropColumn("dbo.DatasetCategoryElement", "Chronic");
            DropColumn("dbo.DatasetCategoryElement", "Acute");
        }
    }
}
