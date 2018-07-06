namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetaPageToWidget : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.MetaWidget", new[] { "MetaPage_Id" });
            AlterColumn("dbo.MetaWidget", "MetaPage_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.MetaWidget", "MetaPage_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.MetaWidget", new[] { "MetaPage_Id" });
            AlterColumn("dbo.MetaWidget", "MetaPage_Id", c => c.Int());
            CreateIndex("dbo.MetaWidget", "MetaPage_Id");
        }
    }
}
