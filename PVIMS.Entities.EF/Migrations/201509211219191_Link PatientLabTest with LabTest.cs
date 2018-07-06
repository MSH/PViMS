namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkPatientLabTestwithLabTest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientLabTest", "TestUnit_Id", c => c.Int());
            CreateIndex("dbo.PatientLabTest", "TestUnit_Id");
            AddForeignKey("dbo.PatientLabTest", "TestUnit_Id", "dbo.LabTestUnit", "Id");
            DropColumn("dbo.PatientLabTest", "TestUnit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientLabTest", "TestUnit", c => c.String(maxLength: 20));
            DropForeignKey("dbo.PatientLabTest", "TestUnit_Id", "dbo.LabTestUnit");
            DropIndex("dbo.PatientLabTest", new[] { "TestUnit_Id" });
            DropColumn("dbo.PatientLabTest", "TestUnit_Id");
        }
    }
}
