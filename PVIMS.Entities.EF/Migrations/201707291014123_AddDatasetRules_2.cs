namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatasetRules_2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DatasetRule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RuleType = c.Int(nullable: false),
                        RuleActive = c.Boolean(nullable: false),
                        Dataset_Id = c.Int(),
                        DatasetElement_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dataset", t => t.Dataset_Id)
                .ForeignKey("dbo.DatasetElement", t => t.DatasetElement_Id)
                .Index(t => t.Dataset_Id)
                .Index(t => t.DatasetElement_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DatasetRule", "DatasetElement_Id", "dbo.DatasetElement");
            DropForeignKey("dbo.DatasetRule", "Dataset_Id", "dbo.Dataset");
            DropIndex("dbo.DatasetRule", new[] { "DatasetElement_Id" });
            DropIndex("dbo.DatasetRule", new[] { "Dataset_Id" });
            DropTable("dbo.DatasetRule");
        }
    }
}
