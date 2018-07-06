namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserFacility : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserFacility",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Facility_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Facility", t => t.Facility_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.Facility_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserFacility", "User_Id", "dbo.User");
            DropForeignKey("dbo.UserFacility", "Facility_Id", "dbo.Facility");
            DropIndex("dbo.UserFacility", new[] { "User_Id" });
            DropIndex("dbo.UserFacility", new[] { "Facility_Id" });
            DropTable("dbo.UserFacility");
        }
    }
}
