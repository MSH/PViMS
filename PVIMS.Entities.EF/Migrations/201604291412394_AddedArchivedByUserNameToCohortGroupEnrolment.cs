namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedArchivedByUserNameToCohortGroupEnrolment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CohortGroupEnrolment", "ArchivedByUserName", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CohortGroupEnrolment", "ArchivedByUserName");
        }
    }
}
