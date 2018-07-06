namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameDateEndtoOutcomeDate : DbMigration
    {
        public override void Up()
        {
           
            RenameColumn("dbo.PatientCondition", "DateEnd", "OutcomeDate");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.PatientCondition","OutcomeDate", "DateEnd" );
        }
    }
}
