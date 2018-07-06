namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class E2BCriteriaDatasetElement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetElement", "OID", c => c.String(maxLength: 50));
            AddColumn("dbo.DatasetElement", "DefaultValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetElement", "DefaultValue");
            DropColumn("dbo.DatasetElement", "OID");
        }
    }
}
