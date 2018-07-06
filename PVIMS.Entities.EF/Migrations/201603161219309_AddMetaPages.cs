namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetaPages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MetaPage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metapage_guid = c.Guid(nullable: false),
                        PageName = c.String(nullable: false, maxLength: 50),
                        PageDefinition = c.String(maxLength: 250),
                        MetaDefinition = c.String(),
                        Breadcrumb = c.String(maxLength: 250),
                        IsSystem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MetaWidget",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metawidget_guid = c.Guid(nullable: false),
                        WidgetName = c.String(nullable: false, maxLength: 50),
                        WidgetDefinition = c.String(maxLength: 250),
                        Content = c.String(),
                        WidgetType_Id = c.Int(nullable: false),
                        MetaPage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MetaWidgetType", t => t.WidgetType_Id)
                .ForeignKey("dbo.MetaPage", t => t.MetaPage_Id)
                .Index(t => t.WidgetType_Id)
                .Index(t => t.MetaPage_Id);
            
            CreateTable(
                "dbo.MetaWidgetType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metawidgettype_guid = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MetaWidget", "MetaPage_Id", "dbo.MetaPage");
            DropForeignKey("dbo.MetaWidget", "WidgetType_Id", "dbo.MetaWidgetType");
            DropIndex("dbo.MetaWidget", new[] { "MetaPage_Id" });
            DropIndex("dbo.MetaWidget", new[] { "WidgetType_Id" });
            DropTable("dbo.MetaWidgetType");
            DropTable("dbo.MetaWidget");
            DropTable("dbo.MetaPage");
        }
    }
}
