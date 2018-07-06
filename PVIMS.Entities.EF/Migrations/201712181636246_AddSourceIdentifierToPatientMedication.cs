namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSourceIdentifierToPatientMedication : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportInstance", "SourceIdentifier", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportInstance", "SourceIdentifier");
        }
    }
}
