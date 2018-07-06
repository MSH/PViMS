namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTreatmentStartDateFromCondition : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PatientCondition", "TreatmentStartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientCondition", "TreatmentStartDate", c => c.DateTime());
        }
    }
}
