namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkedUserToPatientRelatedTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointment", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.Appointment", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.Appointment", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.Appointment", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.Encounter", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.Encounter", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.Encounter", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.Encounter", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.Attachment", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.Attachment", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.Attachment", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.Attachment", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.Patient", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.Patient", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.Patient", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.Patient", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.PatientClinicalEvent", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientClinicalEvent", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.PatientClinicalEvent", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientClinicalEvent", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.MedicationCausality", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.MedicationCausality", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.MedicationCausality", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.MedicationCausality", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.PatientMedication", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientMedication", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.PatientMedication", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientMedication", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.PatientLabTest", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientLabTest", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.PatientLabTest", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientLabTest", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.PatientCondition", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientCondition", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.PatientCondition", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientCondition", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.PatientFacility", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientFacility", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.PatientFacility", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientFacility", "AuditUser_Id", c => c.Int());
            AddColumn("dbo.PatientStatusHistory", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientStatusHistory", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.PatientStatusHistory", "ArchivedReason", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientStatusHistory", "AuditUser_Id", c => c.Int());
            CreateIndex("dbo.Appointment", "AuditUser_Id");
            CreateIndex("dbo.Attachment", "AuditUser_Id");
            CreateIndex("dbo.Encounter", "AuditUser_Id");
            CreateIndex("dbo.Patient", "AuditUser_Id");
            CreateIndex("dbo.PatientClinicalEvent", "AuditUser_Id");
            CreateIndex("dbo.MedicationCausality", "AuditUser_Id");
            CreateIndex("dbo.PatientMedication", "AuditUser_Id");
            CreateIndex("dbo.PatientLabTest", "AuditUser_Id");
            CreateIndex("dbo.PatientCondition", "AuditUser_Id");
            CreateIndex("dbo.PatientFacility", "AuditUser_Id");
            CreateIndex("dbo.PatientStatusHistory", "AuditUser_Id");
            AddForeignKey("dbo.Attachment", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.Encounter", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.Patient", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.PatientClinicalEvent", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.MedicationCausality", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.PatientMedication", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.PatientLabTest", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.PatientCondition", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.PatientFacility", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.PatientStatusHistory", "AuditUser_Id", "dbo.User", "Id");
            AddForeignKey("dbo.Appointment", "AuditUser_Id", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointment", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.PatientStatusHistory", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.PatientFacility", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.PatientCondition", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.PatientLabTest", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.PatientMedication", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.MedicationCausality", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.PatientClinicalEvent", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.Patient", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.Encounter", "AuditUser_Id", "dbo.User");
            DropForeignKey("dbo.Attachment", "AuditUser_Id", "dbo.User");
            DropIndex("dbo.PatientStatusHistory", new[] { "AuditUser_Id" });
            DropIndex("dbo.PatientFacility", new[] { "AuditUser_Id" });
            DropIndex("dbo.PatientCondition", new[] { "AuditUser_Id" });
            DropIndex("dbo.PatientLabTest", new[] { "AuditUser_Id" });
            DropIndex("dbo.PatientMedication", new[] { "AuditUser_Id" });
            DropIndex("dbo.MedicationCausality", new[] { "AuditUser_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "AuditUser_Id" });
            DropIndex("dbo.Patient", new[] { "AuditUser_Id" });
            DropIndex("dbo.Encounter", new[] { "AuditUser_Id" });
            DropIndex("dbo.Attachment", new[] { "AuditUser_Id" });
            DropIndex("dbo.Appointment", new[] { "AuditUser_Id" });
            DropColumn("dbo.PatientStatusHistory", "AuditUser_Id");
            DropColumn("dbo.PatientStatusHistory", "ArchivedReason");
            DropColumn("dbo.PatientStatusHistory", "ArchivedDate");
            DropColumn("dbo.PatientStatusHistory", "Archived");
            DropColumn("dbo.PatientFacility", "AuditUser_Id");
            DropColumn("dbo.PatientFacility", "ArchivedReason");
            DropColumn("dbo.PatientFacility", "ArchivedDate");
            DropColumn("dbo.PatientFacility", "Archived");
            DropColumn("dbo.PatientCondition", "AuditUser_Id");
            DropColumn("dbo.PatientCondition", "ArchivedReason");
            DropColumn("dbo.PatientCondition", "ArchivedDate");
            DropColumn("dbo.PatientCondition", "Archived");
            DropColumn("dbo.PatientLabTest", "AuditUser_Id");
            DropColumn("dbo.PatientLabTest", "ArchivedReason");
            DropColumn("dbo.PatientLabTest", "ArchivedDate");
            DropColumn("dbo.PatientLabTest", "Archived");
            DropColumn("dbo.PatientMedication", "AuditUser_Id");
            DropColumn("dbo.PatientMedication", "ArchivedReason");
            DropColumn("dbo.PatientMedication", "ArchivedDate");
            DropColumn("dbo.PatientMedication", "Archived");
            DropColumn("dbo.MedicationCausality", "AuditUser_Id");
            DropColumn("dbo.MedicationCausality", "ArchivedReason");
            DropColumn("dbo.MedicationCausality", "ArchivedDate");
            DropColumn("dbo.MedicationCausality", "Archived");
            DropColumn("dbo.PatientClinicalEvent", "AuditUser_Id");
            DropColumn("dbo.PatientClinicalEvent", "ArchivedReason");
            DropColumn("dbo.PatientClinicalEvent", "ArchivedDate");
            DropColumn("dbo.PatientClinicalEvent", "Archived");
            DropColumn("dbo.Patient", "AuditUser_Id");
            DropColumn("dbo.Patient", "ArchivedReason");
            DropColumn("dbo.Patient", "ArchivedDate");
            DropColumn("dbo.Patient", "Archived");
            DropColumn("dbo.Attachment", "AuditUser_Id");
            DropColumn("dbo.Attachment", "ArchivedReason");
            DropColumn("dbo.Attachment", "ArchivedDate");
            DropColumn("dbo.Attachment", "Archived");
            DropColumn("dbo.Encounter", "AuditUser_Id");
            DropColumn("dbo.Encounter", "ArchivedReason");
            DropColumn("dbo.Encounter", "ArchivedDate");
            DropColumn("dbo.Encounter", "Archived");
            DropColumn("dbo.Appointment", "AuditUser_Id");
            DropColumn("dbo.Appointment", "ArchivedReason");
            DropColumn("dbo.Appointment", "ArchivedDate");
            DropColumn("dbo.Appointment", "Archived");
        }
    }
}
