namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHelpToElementSubs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetElementSub", "FriendlyName", c => c.String(maxLength: 150));
            AddColumn("dbo.DatasetElementSub", "Help", c => c.String(maxLength: 350));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetElementSub", "Help");
            DropColumn("dbo.DatasetElementSub", "FriendlyName");
        }
    }
}
