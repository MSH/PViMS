namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReferenceToLabTest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientLabTest", "ReferenceLower", c => c.String(maxLength: 100));
            AddColumn("dbo.PatientLabTest", "ReferenceUpper", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientLabTest", "ReferenceUpper");
            DropColumn("dbo.PatientLabTest", "ReferenceLower");
        }
    }
}
