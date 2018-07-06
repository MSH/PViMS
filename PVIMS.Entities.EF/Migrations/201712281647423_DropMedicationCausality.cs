namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropMedicationCausality : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MedicationCausality", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.MedicationCausality", "ClinicalEvent_Id", "dbo.PatientClinicalEvent");
            DropForeignKey("dbo.MedicationCausality", "Medication_Id", "dbo.PatientMedication");
            DropIndex("dbo.MedicationCausality", new[] { "AuditUser_Id" });
            DropIndex("dbo.MedicationCausality", new[] { "ClinicalEvent_Id" });
            DropIndex("dbo.MedicationCausality", new[] { "Medication_Id" });
            DropTable("dbo.MedicationCausality");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MedicationCausality",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NaranjoCausality = c.String(maxLength: 30),
                        WhoCausality = c.String(maxLength: 30),
                        Archived = c.Boolean(nullable: false),
                        ArchivedDate = c.DateTime(),
                        ArchivedReason = c.String(maxLength: 200),
                        AuditUser_Id = c.Int(),
                        ClinicalEvent_Id = c.Int(nullable: false),
                        Medication_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.MedicationCausality", "Medication_Id");
            CreateIndex("dbo.MedicationCausality", "ClinicalEvent_Id");
            CreateIndex("dbo.MedicationCausality", "AuditUser_Id");
            AddForeignKey("dbo.MedicationCausality", "Medication_Id", "dbo.PatientMedication", "Id");
            AddForeignKey("dbo.MedicationCausality", "ClinicalEvent_Id", "dbo.PatientClinicalEvent", "Id");
            AddForeignKey("dbo.MedicationCausality", "AuditUser_Id", "dbo.User", "Id");
        }
    }
}
