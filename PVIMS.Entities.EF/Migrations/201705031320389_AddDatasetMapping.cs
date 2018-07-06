namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatasetMapping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DatasetMapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tag = c.String(),
                        MappingType = c.String(),
                        MappingOption = c.String(),
                        DestinationElement_Id = c.Int(nullable: false),
                        SourceElement_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetCategoryElement", t => t.DestinationElement_Id)
                .ForeignKey("dbo.DatasetCategoryElement", t => t.SourceElement_Id)
                .Index(t => t.DestinationElement_Id)
                .Index(t => t.SourceElement_Id);
            
            CreateTable(
                "dbo.DatasetMappingValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceValue = c.String(nullable: false, maxLength: 100),
                        DestinationValue = c.String(nullable: false, maxLength: 100),
                        Active = c.Boolean(nullable: false),
                        Mapping_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetMapping", t => t.Mapping_Id)
                .Index(t => t.Mapping_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DatasetMapping", "SourceElement_Id", "dbo.DatasetCategoryElement");
            DropForeignKey("dbo.DatasetMapping", "DestinationElement_Id", "dbo.DatasetCategoryElement");
            DropForeignKey("dbo.DatasetMappingValue", "Mapping_Id", "dbo.DatasetMapping");
            DropIndex("dbo.DatasetMappingValue", new[] { "Mapping_Id" });
            DropIndex("dbo.DatasetMapping", new[] { "SourceElement_Id" });
            DropIndex("dbo.DatasetMapping", new[] { "DestinationElement_Id" });
            DropTable("dbo.DatasetMappingValue");
            DropTable("dbo.DatasetMapping");
        }
    }
}
