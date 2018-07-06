namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedArchiveFieldsToCohortGroupEnrolment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CohortGroupEnrolment", "DeenroledDate", c => c.DateTime());
            AddColumn("dbo.CohortGroupEnrolment", "Archived", c => c.Boolean(nullable: false));
            AddColumn("dbo.CohortGroupEnrolment", "ArchivedDate", c => c.DateTime());
            AddColumn("dbo.CohortGroupEnrolment", "ArchivedReason", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CohortGroupEnrolment", "ArchivedReason");
            DropColumn("dbo.CohortGroupEnrolment", "ArchivedDate");
            DropColumn("dbo.CohortGroupEnrolment", "Archived");
            DropColumn("dbo.CohortGroupEnrolment", "DeenroledDate");
        }
    }
}
