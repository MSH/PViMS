namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportingInstanceMedication : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PatientClinicalEvent", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropIndex("dbo.PatientClinicalEvent", new[] { "TerminologyMedDra_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "TerminologyMedDra_Id1" });
            CreateTable(
                "dbo.ReportInstanceMedication",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MedicationIdentifier = c.String(),
                        NaranjoCausality = c.String(maxLength: 30),
                        WhoCausality = c.String(maxLength: 30),
                        Medication_Id = c.Int(),
                        ReportInstance_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Medication", t => t.Medication_Id)
                .ForeignKey("dbo.ReportInstance", t => t.ReportInstance_Id)
                .Index(t => t.Medication_Id)
                .Index(t => t.ReportInstance_Id);
            
            AddColumn("dbo.ReportInstance", "PatientIdentifier", c => c.String(nullable: false));
            AddColumn("dbo.ReportInstance", "TerminologyMedDra_Id", c => c.Int());
            CreateIndex("dbo.ReportInstance", "TerminologyMedDra_Id");
            AddForeignKey("dbo.ReportInstance", "TerminologyMedDra_Id", "dbo.TerminologyMedDra", "Id");
            DropColumn("dbo.PatientClinicalEvent", "TerminologyMedDra_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientClinicalEvent", "TerminologyMedDra_Id", c => c.Int());
            DropForeignKey("dbo.ReportInstance", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropForeignKey("dbo.ReportInstanceMedication", "ReportInstance_Id", "dbo.ReportInstance");
            DropForeignKey("dbo.ReportInstanceMedication", "Medication_Id", "dbo.Medication");
            DropIndex("dbo.ReportInstanceMedication", new[] { "ReportInstance_Id" });
            DropIndex("dbo.ReportInstanceMedication", new[] { "Medication_Id" });
            DropIndex("dbo.ReportInstance", new[] { "TerminologyMedDra_Id" });
            DropColumn("dbo.ReportInstance", "TerminologyMedDra_Id");
            DropColumn("dbo.ReportInstance", "PatientIdentifier");
            DropTable("dbo.ReportInstanceMedication");
            CreateIndex("dbo.PatientClinicalEvent", "TerminologyMedDra_Id1");
            CreateIndex("dbo.PatientClinicalEvent", "TerminologyMedDra_Id");
            AddForeignKey("dbo.PatientClinicalEvent", "TerminologyMedDra_Id", "dbo.TerminologyMedDra", "Id");
        }
    }
}
