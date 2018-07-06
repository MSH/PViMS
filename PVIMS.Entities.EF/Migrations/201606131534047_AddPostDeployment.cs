namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPostDeployment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostDeployment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ScriptGuid = c.Guid(nullable: false),
                        ScriptFileName = c.String(nullable: false, maxLength: 200),
                        ScriptDescription = c.String(nullable: false, maxLength: 200),
                        RunDate = c.DateTime(precision: 0, storeType: "datetime2"),
                        StatusCode = c.Int(),
                        StatusMessage = c.String(),
                        RunRank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ScriptFileName, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PostDeployment", new[] { "ScriptFileName" });
            DropTable("dbo.PostDeployment");
        }
    }
}
