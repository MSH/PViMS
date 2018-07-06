namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameDetailsToCommentsInPatientStatusHistory : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.PatientStatusHistory", "Details", "Comments");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.PatientStatusHistory", "Comments", "Details");

        }
    }
}
