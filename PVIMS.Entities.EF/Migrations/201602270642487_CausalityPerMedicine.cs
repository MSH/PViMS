namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CausalityPerMedicine : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PatientClinicalEvent", name: "CausativeMedication_Id", newName: "Medication_Id");
            RenameIndex(table: "dbo.PatientClinicalEvent", name: "IX_CausativeMedication_Id", newName: "IX_Medication_Id");
            CreateTable(
                "dbo.MedicationCausality",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NaranjoCausality = c.String(maxLength: 10),
                        WhoCausality = c.String(maxLength: 30),
                        ClinicalEvent_Id = c.Int(nullable: false),
                        Medication_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PatientClinicalEvent", t => t.ClinicalEvent_Id)
                .ForeignKey("dbo.PatientMedication", t => t.Medication_Id)
                .Index(t => t.ClinicalEvent_Id)
                .Index(t => t.Medication_Id);
            
            DropColumn("dbo.PatientClinicalEvent", "NaranjoCausality");
            DropColumn("dbo.PatientClinicalEvent", "WhoCausality");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientClinicalEvent", "WhoCausality", c => c.String(maxLength: 30));
            AddColumn("dbo.PatientClinicalEvent", "NaranjoCausality", c => c.String(maxLength: 10));
            DropForeignKey("dbo.MedicationCausality", "Medication_Id", "dbo.PatientMedication");
            DropForeignKey("dbo.MedicationCausality", "ClinicalEvent_Id", "dbo.PatientClinicalEvent");
            DropIndex("dbo.MedicationCausality", new[] { "Medication_Id" });
            DropIndex("dbo.MedicationCausality", new[] { "ClinicalEvent_Id" });
            DropTable("dbo.MedicationCausality");
            RenameIndex(table: "dbo.PatientClinicalEvent", name: "IX_Medication_Id", newName: "IX_CausativeMedication_Id");
            RenameColumn(table: "dbo.PatientClinicalEvent", name: "Medication_Id", newName: "CausativeMedication_Id");
        }
    }
}
