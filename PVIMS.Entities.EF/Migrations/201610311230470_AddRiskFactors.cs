namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRiskFactors : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RiskFactorOption",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OptionName = c.String(nullable: false, maxLength: 50),
                        Criteria = c.String(nullable: false, maxLength: 250),
                        Display = c.String(maxLength: 30),
                        RiskFactor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RiskFactor", t => t.RiskFactor_Id)
                .Index(t => t.RiskFactor_Id);
            
            CreateTable(
                "dbo.RiskFactor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FactorName = c.String(nullable: false, maxLength: 50),
                        Criteria = c.String(nullable: false, maxLength: 250),
                        Display = c.String(maxLength: 20),
                        IsSystem = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RiskFactorOption", "RiskFactor_Id", "dbo.RiskFactor");
            DropIndex("dbo.RiskFactorOption", new[] { "RiskFactor_Id" });
            DropTable("dbo.RiskFactor");
            DropTable("dbo.RiskFactorOption");
        }
    }
}
