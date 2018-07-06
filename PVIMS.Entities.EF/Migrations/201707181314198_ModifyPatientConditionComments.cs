namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyPatientConditionComments : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientCondition", "Comments", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientCondition", "Comments", c => c.String(maxLength: 250));
        }
    }
}
