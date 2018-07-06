namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMedDRAScale : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MedDRAGrading", "GradingScale_Id", "dbo.SelectionDataItem");
            DropForeignKey("dbo.MedDRAGrading", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropIndex("dbo.MedDRAGrading", new[] { "GradingScale_Id" });
            DropIndex("dbo.MedDRAGrading", new[] { "TerminologyMedDra_Id" });
            CreateTable(
                "dbo.MedDRAScale",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GradingScale_Id = c.Int(nullable: false),
                        TerminologyMedDra_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SelectionDataItem", t => t.GradingScale_Id)
                .ForeignKey("dbo.TerminologyMedDra", t => t.TerminologyMedDra_Id)
                .Index(t => t.GradingScale_Id)
                .Index(t => t.TerminologyMedDra_Id);
            
            AddColumn("dbo.MedDRAGrading", "Scale_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.MedDRAGrading", "Scale_Id");
            AddForeignKey("dbo.MedDRAGrading", "Scale_Id", "dbo.MedDRAScale", "Id");
            DropColumn("dbo.MedDRAGrading", "GradingScale_Id");
            DropColumn("dbo.MedDRAGrading", "TerminologyMedDra_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MedDRAGrading", "TerminologyMedDra_Id", c => c.Int(nullable: false));
            AddColumn("dbo.MedDRAGrading", "GradingScale_Id", c => c.Int(nullable: false));
            DropForeignKey("dbo.MedDRAScale", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropForeignKey("dbo.MedDRAScale", "GradingScale_Id", "dbo.SelectionDataItem");
            DropForeignKey("dbo.MedDRAGrading", "Scale_Id", "dbo.MedDRAScale");
            DropIndex("dbo.MedDRAGrading", new[] { "Scale_Id" });
            DropIndex("dbo.MedDRAScale", new[] { "TerminologyMedDra_Id" });
            DropIndex("dbo.MedDRAScale", new[] { "GradingScale_Id" });
            DropColumn("dbo.MedDRAGrading", "Scale_Id");
            DropTable("dbo.MedDRAScale");
            CreateIndex("dbo.MedDRAGrading", "TerminologyMedDra_Id");
            CreateIndex("dbo.MedDRAGrading", "GradingScale_Id");
            AddForeignKey("dbo.MedDRAGrading", "TerminologyMedDra_Id", "dbo.TerminologyMedDra", "Id");
            AddForeignKey("dbo.MedDRAGrading", "GradingScale_Id", "dbo.SelectionDataItem", "Id");
        }
    }
}
