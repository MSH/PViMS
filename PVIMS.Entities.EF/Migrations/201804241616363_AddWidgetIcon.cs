namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWidgetIcon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaWidget", "Icon", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaWidget", "Icon");
        }
    }
}
