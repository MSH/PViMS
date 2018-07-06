namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addcurrentcontexttouser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "CurrentContext", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "CurrentContext");
        }
    }
}
