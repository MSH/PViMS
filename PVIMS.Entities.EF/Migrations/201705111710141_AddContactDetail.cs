namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContactDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteContactDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContactType = c.Int(nullable: false),
                        ContactFirstName = c.String(nullable: false, maxLength: 30),
                        ContactSurname = c.String(nullable: false, maxLength: 30),
                        StreetAddress = c.String(nullable: false, maxLength: 100),
                        City = c.String(nullable: false, maxLength: 50),
                        State = c.String(maxLength: 50),
                        PostCode = c.String(maxLength: 20),
                        ContactNumber = c.String(maxLength: 50),
                        ContactEmail = c.String(maxLength: 50),
                        CountryCode = c.String(maxLength: 10),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.UpdatedBy_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiteContactDetail", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.SiteContactDetail", "CreatedBy_Id", "dbo.User");
            DropIndex("dbo.SiteContactDetail", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.SiteContactDetail", new[] { "CreatedBy_Id" });
            DropTable("dbo.SiteContactDetail");
        }
    }
}
