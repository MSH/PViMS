namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomAttributeChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomAttributeConfiguration2",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ExtendableTypeName = c.String(),
                    CustomAttributeType = c.Int(nullable: false),
                    Category = c.String(),
                    AttributeKey = c.String(),
                })
                .PrimaryKey(t => t.Id);

            Sql("SET IDENTITY_INSERT dbo.[CustomAttributeConfiguration2] ON; INSERT dbo.[CustomAttributeConfiguration2](Id, [ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey]) SELECT Id, [ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey] FROM dbo.[CustomAttributeConfiguration]; SET IDENTITY_INSERT dbo.[CustomAttributeConfiguration2] OFF;");

            DropTable("dbo.CustomAttributeConfiguration");

            RenameTable("dbo.CustomAttributeConfiguration2", "CustomAttributeConfiguration");

            CreateTable(
                "dbo.SelectionDataItem2",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    AttributeKey = c.String(),
                    SelectionKey = c.String(),
                    Value = c.String(),
                })
                .PrimaryKey(t => t.Id);

            Sql("SET IDENTITY_INSERT dbo.[SelectionDataItem2] ON; INSERT dbo.[SelectionDataItem2](Id, [AttributeKey], [SelectionKey], [Value]) SELECT Id, [AttributeKey], [SelectionKey], [Value] FROM dbo.[SelectionDataItem]; SET IDENTITY_INSERT dbo.[SelectionDataItem2] OFF;");

            DropTable("dbo.SelectionDataItem");

            RenameTable("dbo.SelectionDataItem2", "SelectionDataItem");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.CustomAttributeConfiguration2",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    ExtendableTypeName = c.String(),
                    CustomAttributeType = c.Int(nullable: false),
                    Category = c.String(),
                    AttributeKey = c.String(),
                })
                .PrimaryKey(t => t.Id);

            Sql("SET IDENTITY_INSERT dbo.[CustomAttributeConfiguration2] ON; INSERT dbo.[CustomAttributeConfiguration2](Id, [ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey]) SELECT Id, [ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey] FROM dbo.[CustomAttributeConfiguration]; SET IDENTITY_INSERT dbo.[CustomAttributeConfiguration2] OFF;");

            DropTable("dbo.CustomAttributeConfiguration");

            RenameTable("dbo.CustomAttributeConfiguration2", "CustomAttributeConfiguration");

            CreateTable(
                "dbo.SelectionDataItem2",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    AttributeKey = c.String(),
                    SelectionKey = c.String(),
                    Value = c.String(),
                })
                .PrimaryKey(t => t.Id);

            Sql("SET IDENTITY_INSERT dbo.[SelectionDataItem2] ON; INSERT dbo.[SelectionDataItem2](Id, [AttributeKey], [SelectionKey], [Value]) SELECT Id, [AttributeKey], [SelectionKey], [Value] FROM dbo.[SelectionDataItem]; SET IDENTITY_INSERT dbo.[SelectionDataItem2] OFF;");

            DropTable("dbo.SelectionDataItem");

            RenameTable("dbo.SelectionDataItem2", "SelectionDataItem");
        }
    }
}
