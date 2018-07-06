namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adduserandroletables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Key = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Role_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Role", t => t.Role_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.Role_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.User", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "User_Id", "dbo.User");
            DropForeignKey("dbo.UserRole", "Role_Id", "dbo.Role");
            DropIndex("dbo.UserRole", new[] { "User_Id" });
            DropIndex("dbo.UserRole", new[] { "Role_Id" });
            DropColumn("dbo.User", "Active");
            DropTable("dbo.UserRole");
            DropTable("dbo.Role");
        }
    }
}
