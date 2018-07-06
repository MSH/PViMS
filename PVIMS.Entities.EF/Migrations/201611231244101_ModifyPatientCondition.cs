namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyPatientCondition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TreatmentOutcome",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PatientCondition", "TreatmentOutcome_Id", c => c.Int());
            AlterColumn("dbo.PatientCondition", "DateStart", c => c.DateTime(nullable: false));
            CreateIndex("dbo.PatientCondition", "TreatmentOutcome_Id");
            AddForeignKey("dbo.PatientCondition", "TreatmentOutcome_Id", "dbo.TreatmentOutcome", "Id");
            DropColumn("dbo.PatientCondition", "TreatmentStartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientCondition", "TreatmentStartDate", c => c.DateTime());
            DropForeignKey("dbo.PatientCondition", "TreatmentOutcome_Id", "dbo.TreatmentOutcome");
            DropIndex("dbo.PatientCondition", new[] { "TreatmentOutcome_Id" });
            AlterColumn("dbo.PatientCondition", "DateStart", c => c.DateTime());
            DropColumn("dbo.PatientCondition", "TreatmentOutcome_Id");
            DropTable("dbo.TreatmentOutcome");
        }
    }
}
