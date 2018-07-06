namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class E2BCriteriaDatasetElementSub : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetElementSub", "OID", c => c.String(maxLength: 50));
            AddColumn("dbo.DatasetElementSub", "DefaultValue", c => c.String());
            AddColumn("dbo.DatasetElementSub", "System", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetElementSub", "System");
            DropColumn("dbo.DatasetElementSub", "DefaultValue");
            DropColumn("dbo.DatasetElementSub", "OID");
        }
    }
}
