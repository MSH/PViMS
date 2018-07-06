namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkedUserToCohortGroupEnrolment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CohortGroupEnrolment", "AuditUser_Id", c => c.Int());
            CreateIndex("dbo.CohortGroupEnrolment", "AuditUser_Id");
            AddForeignKey("dbo.CohortGroupEnrolment", "AuditUser_Id", "dbo.User", "Id");
            DropColumn("dbo.CohortGroupEnrolment", "ArchivedByUserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CohortGroupEnrolment", "ArchivedByUserName", c => c.String(maxLength: 256));
            DropForeignKey("dbo.CohortGroupEnrolment", "AuditUser_Id", "dbo.User");
            DropIndex("dbo.CohortGroupEnrolment", new[] { "AuditUser_Id" });
            DropColumn("dbo.CohortGroupEnrolment", "AuditUser_Id");
        }
    }
}
