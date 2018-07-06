namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConditionMedDra : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConditionMedDra",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Condition_Id = c.Int(nullable: false),
                        TerminologyMedDra_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Condition", t => t.Condition_Id)
                .ForeignKey("dbo.TerminologyMedDra", t => t.TerminologyMedDra_Id)
                .Index(t => t.Condition_Id)
                .Index(t => t.TerminologyMedDra_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConditionMedDra", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropForeignKey("dbo.ConditionMedDra", "Condition_Id", "dbo.Condition");
            DropIndex("dbo.ConditionMedDra", new[] { "TerminologyMedDra_Id" });
            DropIndex("dbo.ConditionMedDra", new[] { "Condition_Id" });
            DropTable("dbo.ConditionMedDra");
        }
    }
}
