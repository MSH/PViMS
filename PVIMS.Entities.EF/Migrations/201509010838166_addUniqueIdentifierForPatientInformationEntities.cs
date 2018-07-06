namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUniqueIdentifierForPatientInformationEntities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientClinicalEvent", "PatientClinicalEventGuid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AddColumn("dbo.PatientCondition", "PatientConditionGuid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AddColumn("dbo.PatientMedication", "PatientMedicationGuid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AddColumn("dbo.PatientLabTest", "PatientLabTestGuid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientLabTest", "PatientLabTestGuid");
            DropColumn("dbo.PatientMedication", "PatientMedicationGuid");
            DropColumn("dbo.PatientCondition", "PatientConditionGuid");
            DropColumn("dbo.PatientClinicalEvent", "PatientClinicalEventGuid");
        }
    }
}
