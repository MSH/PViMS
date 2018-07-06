namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetaTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MetaColumn",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metacolumn_guid = c.Guid(nullable: false),
                        ColumnName = c.String(nullable: false, maxLength: 50),
                        IsIdentity = c.Boolean(nullable: false),
                        IsNullable = c.Boolean(nullable: false),
                        ColumnType_Id = c.Int(nullable: false),
                        Table_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MetaColumnType", t => t.ColumnType_Id)
                .ForeignKey("dbo.MetaTable", t => t.Table_Id)
                .Index(t => t.ColumnType_Id)
                .Index(t => t.Table_Id);
            
            CreateTable(
                "dbo.MetaColumnType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metacolumntype_guid = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MetaTable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metatable_guid = c.Guid(nullable: false),
                        object_id = c.Int(nullable: false),
                        TableName = c.String(nullable: false, maxLength: 50),
                        FriendlyName = c.String(maxLength: 100),
                        FriendlyDescription = c.String(maxLength: 250),
                        TableType_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MetaTableType", t => t.TableType_Id)
                .Index(t => t.TableType_Id);
            
            CreateTable(
                "dbo.MetaTableType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metatabletype_guid = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MetaDependency",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metadependency_guid = c.Guid(nullable: false),
                        ParentColumnName = c.String(nullable: false, maxLength: 50),
                        ReferenceColumnName = c.String(nullable: false, maxLength: 50),
                        ParentTable_Id = c.Int(nullable: false),
                        ReferenceTable_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MetaTable", t => t.ParentTable_Id)
                .ForeignKey("dbo.MetaTable", t => t.ReferenceTable_Id)
                .Index(t => t.ParentTable_Id)
                .Index(t => t.ReferenceTable_Id);
            
            CreateTable(
                "dbo.MetaReport",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metareport_guid = c.Guid(nullable: false),
                        ReportName = c.String(nullable: false, maxLength: 50),
                        ReportDefinition = c.String(maxLength: 250),
                        MetaDefinition = c.String(),
                        Breadcrumb = c.String(maxLength: 250),
                        SQLDefinition = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MetaDependency", "ReferenceTable_Id", "dbo.MetaTable");
            DropForeignKey("dbo.MetaDependency", "ParentTable_Id", "dbo.MetaTable");
            DropForeignKey("dbo.MetaColumn", "Table_Id", "dbo.MetaTable");
            DropForeignKey("dbo.MetaTable", "TableType_Id", "dbo.MetaTableType");
            DropForeignKey("dbo.MetaColumn", "ColumnType_Id", "dbo.MetaColumnType");
            DropIndex("dbo.MetaDependency", new[] { "ReferenceTable_Id" });
            DropIndex("dbo.MetaDependency", new[] { "ParentTable_Id" });
            DropIndex("dbo.MetaTable", new[] { "TableType_Id" });
            DropIndex("dbo.MetaColumn", new[] { "Table_Id" });
            DropIndex("dbo.MetaColumn", new[] { "ColumnType_Id" });
            DropTable("dbo.MetaReport");
            DropTable("dbo.MetaDependency");
            DropTable("dbo.MetaTableType");
            DropTable("dbo.MetaTable");
            DropTable("dbo.MetaColumnType");
            DropTable("dbo.MetaColumn");
        }
    }
}
