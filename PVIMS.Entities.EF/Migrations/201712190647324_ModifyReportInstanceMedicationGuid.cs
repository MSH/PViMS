namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyReportInstanceMedicationGuid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReportInstanceMedication", "Medication_Id", "dbo.Medication");
            DropIndex("dbo.ReportInstanceMedication", new[] { "Medication_Id" });
            AddColumn("dbo.ReportInstanceMedication", "ReportInstanceMedicationGuid", c => c.Guid(nullable: false));
            DropColumn("dbo.ReportInstanceMedication", "Medication_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReportInstanceMedication", "Medication_Id", c => c.Int());
            DropColumn("dbo.ReportInstanceMedication", "ReportInstanceMedicationGuid");
            CreateIndex("dbo.ReportInstanceMedication", "Medication_Id");
            AddForeignKey("dbo.ReportInstanceMedication", "Medication_Id", "dbo.Medication", "Id");
        }
    }
}
