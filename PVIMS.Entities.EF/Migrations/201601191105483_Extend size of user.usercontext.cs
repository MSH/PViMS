namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Extendsizeofuserusercontext : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "CurrentContext", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "CurrentContext", c => c.String(maxLength: 10));
        }
    }
}
