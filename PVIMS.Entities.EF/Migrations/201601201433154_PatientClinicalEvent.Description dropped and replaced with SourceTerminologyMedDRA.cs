namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatientClinicalEventDescriptiondroppedandreplacedwithSourceTerminologyMedDRA : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PatientClinicalEvent", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            AddColumn("dbo.PatientClinicalEvent", "SourceTerminologyMedDra_Id", c => c.Int());
            AddColumn("dbo.PatientClinicalEvent", "TerminologyMedDra_Id1", c => c.Int());
            CreateIndex("dbo.PatientClinicalEvent", "SourceTerminologyMedDra_Id");
            CreateIndex("dbo.PatientClinicalEvent", "TerminologyMedDra_Id1");
            AddForeignKey("dbo.PatientClinicalEvent", "SourceTerminologyMedDra_Id", "dbo.TerminologyMedDra", "Id");
            AddForeignKey("dbo.PatientClinicalEvent", "TerminologyMedDra_Id1", "dbo.TerminologyMedDra", "Id");
            DropColumn("dbo.PatientClinicalEvent", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientClinicalEvent", "Description", c => c.String(maxLength: 100));
            DropForeignKey("dbo.PatientClinicalEvent", "TerminologyMedDra_Id1", "dbo.TerminologyMedDra");
            DropForeignKey("dbo.PatientClinicalEvent", "SourceTerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropIndex("dbo.PatientClinicalEvent", new[] { "TerminologyMedDra_Id1" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "SourceTerminologyMedDra_Id" });
            DropColumn("dbo.PatientClinicalEvent", "TerminologyMedDra_Id1");
            DropColumn("dbo.PatientClinicalEvent", "SourceTerminologyMedDra_Id");
            AddForeignKey("dbo.PatientClinicalEvent", "TerminologyMedDra_Id", "dbo.TerminologyMedDra", "Id");
        }
    }
}
