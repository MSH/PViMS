namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeNaranjoCausalitySize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MedicationCausality", "NaranjoCausality", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MedicationCausality", "NaranjoCausality", c => c.String(maxLength: 10));
        }
    }
}
