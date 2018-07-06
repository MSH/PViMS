namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAuditLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuditLog", "Log", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuditLog", "Log");
        }
    }
}
