namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusToInstance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetInstance", "InstanceStatus", c => c.Int(nullable: false));
            AddColumn("dbo.DatasetInstance", "ProcessStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetInstance", "ProcessStatus");
            DropColumn("dbo.DatasetInstance", "InstanceStatus");
        }
    }
}
