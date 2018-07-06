namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMedDRAGrading : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedDRAGrading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Grade = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 100),
                        GradingScale_Id = c.Int(nullable: false),
                        TerminologyMedDra_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SelectionDataItem", t => t.GradingScale_Id)
                .ForeignKey("dbo.TerminologyMedDra", t => t.TerminologyMedDra_Id)
                .Index(t => t.GradingScale_Id)
                .Index(t => t.TerminologyMedDra_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MedDRAGrading", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropForeignKey("dbo.MedDRAGrading", "GradingScale_Id", "dbo.SelectionDataItem");
            DropIndex("dbo.MedDRAGrading", new[] { "TerminologyMedDra_Id" });
            DropIndex("dbo.MedDRAGrading", new[] { "GradingScale_Id" });
            DropTable("dbo.MedDRAGrading");
        }
    }
}
