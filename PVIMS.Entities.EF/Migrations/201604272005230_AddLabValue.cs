namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLabValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientLabTest", "LabValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientLabTest", "LabValue");
        }
    }
}
