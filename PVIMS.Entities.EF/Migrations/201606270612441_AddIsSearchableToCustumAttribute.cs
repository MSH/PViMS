namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsSearchableToCustumAttribute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomAttributeConfiguration", "IsSearchable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomAttributeConfiguration", "IsSearchable");
        }
    }
}
