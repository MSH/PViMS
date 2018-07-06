namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatientLabTestNullableFields : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PatientLabTest", new[] { "LabTest_Id" });
            DropIndex("dbo.PatientLabTest", new[] { "Patient_Id" });
            AlterColumn("dbo.PatientLabTest", "LabTest_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.PatientLabTest", "Patient_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PatientLabTest", "LabTest_Id");
            CreateIndex("dbo.PatientLabTest", "Patient_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PatientLabTest", new[] { "Patient_Id" });
            DropIndex("dbo.PatientLabTest", new[] { "LabTest_Id" });
            AlterColumn("dbo.PatientLabTest", "Patient_Id", c => c.Int());
            AlterColumn("dbo.PatientLabTest", "LabTest_Id", c => c.Int());
            CreateIndex("dbo.PatientLabTest", "Patient_Id");
            CreateIndex("dbo.PatientLabTest", "LabTest_Id");
        }
    }
}
