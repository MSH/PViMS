namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVisibleToMetaPage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaPage", "IsVisible", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaPage", "IsVisible");
        }
    }
}
