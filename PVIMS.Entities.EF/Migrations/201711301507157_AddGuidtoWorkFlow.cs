namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuidtoWorkFlow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkFlow", "WorkFlowGuid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkFlow", "WorkFlowGuid");
        }
    }
}
