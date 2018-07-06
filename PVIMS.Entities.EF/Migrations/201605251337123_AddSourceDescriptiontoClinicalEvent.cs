namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSourceDescriptiontoClinicalEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientClinicalEvent", "SourceDescription", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientClinicalEvent", "SourceDescription");
        }
    }
}
