namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveEncounterFromClinicalEvent : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PatientClinicalEvent", new[] { "Encounter_Id" });
            AlterColumn("dbo.PatientClinicalEvent", "Encounter_Id", c => c.Int());
            CreateIndex("dbo.PatientClinicalEvent", "Encounter_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PatientClinicalEvent", new[] { "Encounter_Id" });
            AlterColumn("dbo.PatientClinicalEvent", "Encounter_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PatientClinicalEvent", "Encounter_Id");
        }
    }
}
