namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyFriendlyNameSize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DatasetCategory", "FriendlyName", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DatasetCategory", "FriendlyName", c => c.String(maxLength: 50));
        }
    }
}
