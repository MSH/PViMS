namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommonToMeddra : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TerminologyMedDra", "Common", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TerminologyMedDra", "Common");
        }
    }
}
