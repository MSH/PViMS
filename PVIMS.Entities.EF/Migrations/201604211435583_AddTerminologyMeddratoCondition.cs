namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTerminologyMeddratoCondition : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PatientCondition", new[] { "Patient_Id" });
            AddColumn("dbo.PatientCondition", "TerminologyMedDra_Id", c => c.Int());
            AlterColumn("dbo.PatientCondition", "Patient_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PatientCondition", "Patient_Id");
            CreateIndex("dbo.PatientCondition", "TerminologyMedDra_Id");
            AddForeignKey("dbo.PatientCondition", "TerminologyMedDra_Id", "dbo.TerminologyMedDra", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientCondition", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropIndex("dbo.PatientCondition", new[] { "TerminologyMedDra_Id" });
            DropIndex("dbo.PatientCondition", new[] { "Patient_Id" });
            AlterColumn("dbo.PatientCondition", "Patient_Id", c => c.Int());
            DropColumn("dbo.PatientCondition", "TerminologyMedDra_Id");
            CreateIndex("dbo.PatientCondition", "Patient_Id");
        }
    }
}
