namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class E2BCriteriaDatasetElementSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetElement", "System", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetElement", "System");
        }
    }
}
