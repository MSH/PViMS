namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnabledLabResultToBeNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientLabTest", "LabValue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientLabTest", "LabValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
