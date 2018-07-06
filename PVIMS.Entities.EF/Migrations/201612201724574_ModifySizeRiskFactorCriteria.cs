namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifySizeRiskFactorCriteria : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RiskFactor", "Criteria", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RiskFactor", "Criteria", c => c.String(nullable: false, maxLength: 250));
        }
    }
}
