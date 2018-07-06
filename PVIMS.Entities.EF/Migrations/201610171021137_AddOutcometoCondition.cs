namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOutcometoCondition : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PatientClinicalEvent", "Outcome_Id", "dbo.Outcome");
            DropIndex("dbo.PatientClinicalEvent", new[] { "Outcome_Id" });
            AddColumn("dbo.PatientCondition", "Outcome_Id", c => c.Int());
            CreateIndex("dbo.PatientCondition", "Outcome_Id");
            AddForeignKey("dbo.PatientCondition", "Outcome_Id", "dbo.Outcome", "Id");
            DropColumn("dbo.PatientClinicalEvent", "Outcome_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientClinicalEvent", "Outcome_Id", c => c.Int());
            DropForeignKey("dbo.PatientCondition", "Outcome_Id", "dbo.Outcome");
            DropIndex("dbo.PatientCondition", new[] { "Outcome_Id" });
            DropColumn("dbo.PatientCondition", "Outcome_Id");
            CreateIndex("dbo.PatientClinicalEvent", "Outcome_Id");
            AddForeignKey("dbo.PatientClinicalEvent", "Outcome_Id", "dbo.Outcome", "Id");
        }
    }
}
