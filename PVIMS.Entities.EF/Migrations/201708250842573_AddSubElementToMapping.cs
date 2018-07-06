namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubElementToMapping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetMapping", "DestinationSubElement_Id", c => c.Int());
            AddColumn("dbo.DatasetMapping", "SourceSubElement_Id", c => c.Int());
            CreateIndex("dbo.DatasetMapping", "DestinationSubElement_Id");
            CreateIndex("dbo.DatasetMapping", "SourceSubElement_Id");
            AddForeignKey("dbo.DatasetMapping", "DestinationSubElement_Id", "dbo.DatasetElementSub", "Id");
            AddForeignKey("dbo.DatasetMapping", "SourceSubElement_Id", "dbo.DatasetElementSub", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DatasetMapping", "SourceSubElement_Id", "dbo.DatasetElementSub");
            DropForeignKey("dbo.DatasetMapping", "DestinationSubElement_Id", "dbo.DatasetElementSub");
            DropIndex("dbo.DatasetMapping", new[] { "SourceSubElement_Id" });
            DropIndex("dbo.DatasetMapping", new[] { "DestinationSubElement_Id" });
            DropColumn("dbo.DatasetMapping", "SourceSubElement_Id");
            DropColumn("dbo.DatasetMapping", "DestinationSubElement_Id");
        }
    }
}
