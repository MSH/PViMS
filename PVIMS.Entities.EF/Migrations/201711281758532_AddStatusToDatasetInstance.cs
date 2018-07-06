namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusToDatasetInstance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetInstance", "DatasetInstanceGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.DatasetInstance", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetInstance", "Status");
            DropColumn("dbo.DatasetInstance", "DatasetInstanceGuid");
        }
    }
}
