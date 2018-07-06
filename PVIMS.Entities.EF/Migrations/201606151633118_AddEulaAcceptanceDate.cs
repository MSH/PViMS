namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEulaAcceptanceDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "EulaAcceptanceDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "EulaAcceptanceDate");
        }
    }
}
