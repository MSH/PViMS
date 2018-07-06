namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderAndLocationToMetaPage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaWidget", "WidgetLocation", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaWidget", "WidgetLocation");
        }
    }
}
