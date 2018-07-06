namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removecontextfromdatasetelementsub : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DatasetElementSub", "Context");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DatasetElementSub", "Context", c => c.Guid(nullable: false));
        }
    }
}
