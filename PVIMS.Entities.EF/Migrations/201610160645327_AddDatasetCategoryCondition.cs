namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatasetCategoryCondition : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DatasetCategoryElementCondition", new[] { "Condition_Id" });
            DropIndex("dbo.DatasetCategoryElementCondition", new[] { "DatasetCategoryElement_Id" });
            CreateTable(
                "dbo.DatasetCategoryCondition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Condition_Id = c.Int(nullable: false),
                        DatasetCategory_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Condition", t => t.Condition_Id)
                .ForeignKey("dbo.DatasetCategory", t => t.DatasetCategory_Id)
                .Index(t => t.Condition_Id)
                .Index(t => t.DatasetCategory_Id);
            
            AddColumn("dbo.DatasetCategory", "System", c => c.Boolean(nullable: false));
            AddColumn("dbo.DatasetCategory", "Acute", c => c.Boolean(nullable: false));
            AddColumn("dbo.DatasetCategory", "Chronic", c => c.Boolean(nullable: false));
            AlterColumn("dbo.DatasetCategoryElementCondition", "Condition_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.DatasetCategoryElementCondition", "DatasetCategoryElement_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.DatasetCategoryElementCondition", "Condition_Id");
            CreateIndex("dbo.DatasetCategoryElementCondition", "DatasetCategoryElement_Id");
            DropColumn("dbo.DatasetCategory", "IsSystem");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DatasetCategory", "IsSystem", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.DatasetCategoryCondition", "DatasetCategory_Id", "dbo.DatasetCategory");
            DropForeignKey("dbo.DatasetCategoryCondition", "Condition_Id", "dbo.Condition");
            DropIndex("dbo.DatasetCategoryCondition", new[] { "DatasetCategory_Id" });
            DropIndex("dbo.DatasetCategoryCondition", new[] { "Condition_Id" });
            DropIndex("dbo.DatasetCategoryElementCondition", new[] { "DatasetCategoryElement_Id" });
            DropIndex("dbo.DatasetCategoryElementCondition", new[] { "Condition_Id" });
            AlterColumn("dbo.DatasetCategoryElementCondition", "DatasetCategoryElement_Id", c => c.Int());
            AlterColumn("dbo.DatasetCategoryElementCondition", "Condition_Id", c => c.Int());
            DropColumn("dbo.DatasetCategory", "Chronic");
            DropColumn("dbo.DatasetCategory", "Acute");
            DropColumn("dbo.DatasetCategory", "System");
            DropTable("dbo.DatasetCategoryCondition");
            CreateIndex("dbo.DatasetCategoryElementCondition", "DatasetCategoryElement_Id");
            CreateIndex("dbo.DatasetCategoryElementCondition", "Condition_Id");
        }
    }
}
