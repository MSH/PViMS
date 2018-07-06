namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrganisationToContactDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SiteContactDetail", "OrganisationName", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SiteContactDetail", "OrganisationName");
        }
    }
}
