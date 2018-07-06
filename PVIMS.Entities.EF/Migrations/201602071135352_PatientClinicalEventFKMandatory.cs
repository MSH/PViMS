namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatientClinicalEventFKMandatory : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PatientClinicalEvent", new[] { "Encounter_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "Patient_Id" });
            AlterColumn("dbo.PatientClinicalEvent", "Encounter_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.PatientClinicalEvent", "Patient_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PatientClinicalEvent", "Encounter_Id");
            CreateIndex("dbo.PatientClinicalEvent", "Patient_Id");
            AddForeignKey("dbo.PatientClinicalEvent", "TerminologyMedDra_Id", "dbo.TerminologyMedDra", "Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PatientClinicalEvent", new[] { "Patient_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "Encounter_Id" });
            AlterColumn("dbo.PatientClinicalEvent", "Patient_Id", c => c.Int());
            AlterColumn("dbo.PatientClinicalEvent", "Encounter_Id", c => c.Int());
            CreateIndex("dbo.PatientClinicalEvent", "Patient_Id");
            CreateIndex("dbo.PatientClinicalEvent", "Encounter_Id");
        }
    }
}
