namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWidgetStatusToWidget : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaWidget", "WidgetStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaWidget", "WidgetStatus");
        }
    }
}
