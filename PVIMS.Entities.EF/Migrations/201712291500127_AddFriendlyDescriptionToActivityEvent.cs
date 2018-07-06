namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFriendlyDescriptionToActivityEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityExecutionStatus", "FriendlyDescription", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityExecutionStatus", "FriendlyDescription");
        }
    }
}
