namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuidToDatasetElement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetElement", "DatasetElementGuid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetElement", "DatasetElementGuid");
        }
    }
}
