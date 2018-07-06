namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOutcometoClinicalEvent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Outcome",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PatientClinicalEvent", "Outcome_Id", c => c.Int());
            CreateIndex("dbo.PatientClinicalEvent", "Outcome_Id");
            AddForeignKey("dbo.PatientClinicalEvent", "Outcome_Id", "dbo.Outcome", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientClinicalEvent", "Outcome_Id", "dbo.Outcome");
            DropIndex("dbo.PatientClinicalEvent", new[] { "Outcome_Id" });
            DropColumn("dbo.PatientClinicalEvent", "Outcome_Id");
            DropTable("dbo.Outcome");
        }
    }
}
