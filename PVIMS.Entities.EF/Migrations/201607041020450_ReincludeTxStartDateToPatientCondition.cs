namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReincludeTxStartDateToPatientCondition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientCondition", "TreatmentStartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientCondition", "TreatmentStartDate");
        }
    }
}
