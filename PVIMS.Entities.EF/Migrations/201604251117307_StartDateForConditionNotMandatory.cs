namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StartDateForConditionNotMandatory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientCondition", "DateStart", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientCondition", "DateStart", c => c.DateTime(nullable: false));
        }
    }
}
