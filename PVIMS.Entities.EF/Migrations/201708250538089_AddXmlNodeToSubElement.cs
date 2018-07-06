namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddXmlNodeToSubElement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetXmlNode", "DatasetElementSub_Id", c => c.Int());
            CreateIndex("dbo.DatasetXmlNode", "DatasetElementSub_Id");
            AddForeignKey("dbo.DatasetXmlNode", "DatasetElementSub_Id", "dbo.DatasetElementSub", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DatasetXmlNode", "DatasetElementSub_Id", "dbo.DatasetElementSub");
            DropIndex("dbo.DatasetXmlNode", new[] { "DatasetElementSub_Id" });
            DropColumn("dbo.DatasetXmlNode", "DatasetElementSub_Id");
        }
    }
}
