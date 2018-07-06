namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuditLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuditType = c.Int(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        Details = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuditLog", "User_Id", "dbo.User");
            DropIndex("dbo.AuditLog", new[] { "User_Id" });
            DropTable("dbo.AuditLog");
        }
    }
}
