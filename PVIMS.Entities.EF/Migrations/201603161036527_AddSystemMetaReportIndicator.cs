namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSystemMetaReportIndicator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaReport", "IsSystem", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaReport", "IsSystem");
        }
    }
}
