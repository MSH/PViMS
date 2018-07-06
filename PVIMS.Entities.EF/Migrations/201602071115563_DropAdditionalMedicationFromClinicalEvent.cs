namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropAdditionalMedicationFromClinicalEvent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PatientClinicalEvent", "Medication_Id1", "dbo.Medication");
            DropForeignKey("dbo.PatientClinicalEvent", "Medication_Id", "dbo.Medication");
            DropIndex("dbo.PatientClinicalEvent", "IX_Medication_Id1");
            DropIndex("dbo.PatientClinicalEvent", "IX_Medication_Id");
            DropColumn("dbo.PatientClinicalEvent", "Medication_Id1");
            DropColumn("dbo.PatientClinicalEvent", "Medication_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientClinicalEvent", "Medication_Id", c => c.Int());
            AddColumn("dbo.PatientClinicalEvent", "Medication_Id1", c => c.Int());
            CreateIndex("dbo.PatientClinicalEvent", "Medication_Id1");
            CreateIndex("dbo.PatientClinicalEvent", "Medication_Id");
            AddForeignKey("dbo.PatientClinicalEvent", "Medication_Id", "dbo.Medication", "Id");
        }
    }
}
