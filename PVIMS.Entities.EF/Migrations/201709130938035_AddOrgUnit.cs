namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrgUnit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrgUnit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Parent_Id = c.Int(),
                        OrgUnitType_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrgUnit", t => t.Parent_Id)
                .ForeignKey("dbo.OrgUnitType", t => t.OrgUnitType_Id)
                .Index(t => t.Parent_Id)
                .Index(t => t.OrgUnitType_Id);
            
            CreateTable(
                "dbo.OrgUnitType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrgUnitType", t => t.Parent_Id)
                .Index(t => t.Parent_Id);
            
            AddColumn("dbo.Facility", "OrgUnit_Id", c => c.Int());
            CreateIndex("dbo.Facility", "OrgUnit_Id");
            AddForeignKey("dbo.Facility", "OrgUnit_Id", "dbo.OrgUnit", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Facility", "OrgUnit_Id", "dbo.OrgUnit");
            DropForeignKey("dbo.OrgUnit", "OrgUnitType_Id", "dbo.OrgUnitType");
            DropForeignKey("dbo.OrgUnitType", "Parent_Id", "dbo.OrgUnitType");
            DropForeignKey("dbo.OrgUnit", "Parent_Id", "dbo.OrgUnit");
            DropIndex("dbo.OrgUnitType", new[] { "Parent_Id" });
            DropIndex("dbo.OrgUnit", new[] { "OrgUnitType_Id" });
            DropIndex("dbo.OrgUnit", new[] { "Parent_Id" });
            DropIndex("dbo.Facility", new[] { "OrgUnit_Id" });
            DropColumn("dbo.Facility", "OrgUnit_Id");
            DropTable("dbo.OrgUnitType");
            DropTable("dbo.OrgUnit");
        }
    }
}
