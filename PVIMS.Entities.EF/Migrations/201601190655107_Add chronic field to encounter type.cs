namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addchronicfieldtoencountertype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EncounterType", "Chronic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EncounterType", "Chronic");
        }
    }
}
