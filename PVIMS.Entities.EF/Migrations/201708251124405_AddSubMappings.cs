namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubMappings : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DatasetMapping", "DestinationSubElement_Id", "dbo.DatasetElementSub");
            DropForeignKey("dbo.DatasetMapping", "SourceSubElement_Id", "dbo.DatasetElementSub");
            DropIndex("dbo.DatasetMapping", new[] { "DestinationSubElement_Id" });
            DropIndex("dbo.DatasetMapping", new[] { "SourceSubElement_Id" });
            CreateTable(
                "dbo.DatasetMappingSub",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyPath = c.String(),
                        Property = c.String(),
                        MappingType = c.Int(nullable: false),
                        MappingOption = c.String(),
                        DestinationElement_Id = c.Int(),
                        Mapping_Id = c.Int(nullable: false),
                        SourceElement_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetElementSub", t => t.DestinationElement_Id)
                .ForeignKey("dbo.DatasetMapping", t => t.Mapping_Id)
                .ForeignKey("dbo.DatasetElementSub", t => t.SourceElement_Id)
                .Index(t => t.DestinationElement_Id)
                .Index(t => t.Mapping_Id)
                .Index(t => t.SourceElement_Id);
            
            AddColumn("dbo.DatasetMappingValue", "SubMapping_Id", c => c.Int());
            CreateIndex("dbo.DatasetMappingValue", "SubMapping_Id");
            AddForeignKey("dbo.DatasetMappingValue", "SubMapping_Id", "dbo.DatasetMappingSub", "Id");
            DropColumn("dbo.DatasetMapping", "DestinationSubElement_Id");
            DropColumn("dbo.DatasetMapping", "SourceSubElement_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DatasetMapping", "SourceSubElement_Id", c => c.Int());
            AddColumn("dbo.DatasetMapping", "DestinationSubElement_Id", c => c.Int());
            DropForeignKey("dbo.DatasetMappingValue", "SubMapping_Id", "dbo.DatasetMappingSub");
            DropForeignKey("dbo.DatasetMappingSub", "SourceElement_Id", "dbo.DatasetElementSub");
            DropForeignKey("dbo.DatasetMappingSub", "Mapping_Id", "dbo.DatasetMapping");
            DropForeignKey("dbo.DatasetMappingSub", "DestinationElement_Id", "dbo.DatasetElementSub");
            DropIndex("dbo.DatasetMappingValue", new[] { "SubMapping_Id" });
            DropIndex("dbo.DatasetMappingSub", new[] { "SourceElement_Id" });
            DropIndex("dbo.DatasetMappingSub", new[] { "Mapping_Id" });
            DropIndex("dbo.DatasetMappingSub", new[] { "DestinationElement_Id" });
            DropColumn("dbo.DatasetMappingValue", "SubMapping_Id");
            DropTable("dbo.DatasetMappingSub");
            CreateIndex("dbo.DatasetMapping", "SourceSubElement_Id");
            CreateIndex("dbo.DatasetMapping", "DestinationSubElement_Id");
            AddForeignKey("dbo.DatasetMapping", "SourceSubElement_Id", "dbo.DatasetElementSub", "Id");
            AddForeignKey("dbo.DatasetMapping", "DestinationSubElement_Id", "dbo.DatasetElementSub", "Id");
        }
    }
}
