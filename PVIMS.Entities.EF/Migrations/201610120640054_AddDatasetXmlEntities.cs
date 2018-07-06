namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatasetXmlEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DatasetXml",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 50),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.DatasetXmlNode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NodeName = c.String(nullable: false, maxLength: 50),
                        NodeType = c.Int(nullable: false),
                        NodeValue = c.String(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        ParentNode_Id = c.Int(),
                        CreatedBy_Id = c.Int(),
                        DatasetElement_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                        DatasetXml_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetXmlNode", t => t.ParentNode_Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.DatasetElement", t => t.DatasetElement_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .ForeignKey("dbo.DatasetXml", t => t.DatasetXml_Id)
                .Index(t => t.ParentNode_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.DatasetElement_Id)
                .Index(t => t.UpdatedBy_Id)
                .Index(t => t.DatasetXml_Id);
            
            CreateTable(
                "dbo.DatasetXmlAttribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttributeName = c.String(nullable: false, maxLength: 50),
                        AttributeValue = c.String(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        DatasetElement_Id = c.Int(),
                        ParentNode_Id = c.Int(nullable: false),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.DatasetElement", t => t.DatasetElement_Id)
                .ForeignKey("dbo.DatasetXmlNode", t => t.ParentNode_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.DatasetElement_Id)
                .Index(t => t.ParentNode_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.Config",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConfigType = c.Int(nullable: false),
                        ConfigValue = c.String(nullable: false, maxLength: 100),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.UpdatedBy_Id);
            
            AddColumn("dbo.Dataset", "UID", c => c.String(maxLength: 10));
            AddColumn("dbo.Dataset", "IsSystem", c => c.Boolean(nullable: false));
            AddColumn("dbo.Dataset", "DatasetXml_Id", c => c.Int());
            AddColumn("dbo.DatasetCategory", "UID", c => c.String(maxLength: 10));
            AddColumn("dbo.DatasetCategory", "IsSystem", c => c.Boolean(nullable: false));
            AddColumn("dbo.DatasetCategoryElement", "UID", c => c.String(maxLength: 10));
            AddColumn("dbo.DatasetCategoryElement", "System", c => c.Boolean(nullable: false));
            AddColumn("dbo.DatasetElement", "UID", c => c.String(maxLength: 10));
            CreateIndex("dbo.Dataset", "DatasetXml_Id");
            AddForeignKey("dbo.Dataset", "DatasetXml_Id", "dbo.DatasetXml", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Config", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Config", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Dataset", "DatasetXml_Id", "dbo.DatasetXml");
            DropForeignKey("dbo.DatasetXml", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.DatasetXml", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.DatasetXmlNode", "DatasetXml_Id", "dbo.DatasetXml");
            DropForeignKey("dbo.DatasetXmlNode", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.DatasetXmlAttribute", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.DatasetXmlAttribute", "ParentNode_Id", "dbo.DatasetXmlNode");
            DropForeignKey("dbo.DatasetXmlAttribute", "DatasetElement_Id", "dbo.DatasetElement");
            DropForeignKey("dbo.DatasetXmlAttribute", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.DatasetXmlNode", "DatasetElement_Id", "dbo.DatasetElement");
            DropForeignKey("dbo.DatasetXmlNode", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.DatasetXmlNode", "ParentNode_Id", "dbo.DatasetXmlNode");
            DropIndex("dbo.Config", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.Config", new[] { "CreatedBy_Id" });
            DropIndex("dbo.DatasetXmlAttribute", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.DatasetXmlAttribute", new[] { "ParentNode_Id" });
            DropIndex("dbo.DatasetXmlAttribute", new[] { "DatasetElement_Id" });
            DropIndex("dbo.DatasetXmlAttribute", new[] { "CreatedBy_Id" });
            DropIndex("dbo.DatasetXmlNode", new[] { "DatasetXml_Id" });
            DropIndex("dbo.DatasetXmlNode", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.DatasetXmlNode", new[] { "DatasetElement_Id" });
            DropIndex("dbo.DatasetXmlNode", new[] { "CreatedBy_Id" });
            DropIndex("dbo.DatasetXmlNode", new[] { "ParentNode_Id" });
            DropIndex("dbo.DatasetXml", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.DatasetXml", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Dataset", new[] { "DatasetXml_Id" });
            DropColumn("dbo.DatasetElement", "UID");
            DropColumn("dbo.DatasetCategoryElement", "System");
            DropColumn("dbo.DatasetCategoryElement", "UID");
            DropColumn("dbo.DatasetCategory", "IsSystem");
            DropColumn("dbo.DatasetCategory", "UID");
            DropColumn("dbo.Dataset", "DatasetXml_Id");
            DropColumn("dbo.Dataset", "IsSystem");
            DropColumn("dbo.Dataset", "UID");
            DropTable("dbo.Config");
            DropTable("dbo.DatasetXmlAttribute");
            DropTable("dbo.DatasetXmlNode");
            DropTable("dbo.DatasetXml");
        }
    }
}
