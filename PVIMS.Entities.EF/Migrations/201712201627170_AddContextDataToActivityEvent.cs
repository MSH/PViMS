namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContextDataToActivityEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityExecutionStatusEvent", "ContextDateTime", c => c.DateTime());
            AddColumn("dbo.ActivityExecutionStatusEvent", "ContextCode", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityExecutionStatusEvent", "ContextCode");
            DropColumn("dbo.ActivityExecutionStatusEvent", "ContextDateTime");
        }
    }
}
