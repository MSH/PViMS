namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Conditionsperelement : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PatientClinicalEvent", name: "Medication_Id1", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.PatientClinicalEvent", name: "Medication_Id", newName: "Medication_Id1");
            RenameColumn(table: "dbo.PatientClinicalEvent", name: "__mig_tmp__0", newName: "Medication_Id");
            RenameIndex(table: "dbo.PatientClinicalEvent", name: "IX_Medication_Id1", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.PatientClinicalEvent", name: "IX_Medication_Id", newName: "IX_Medication_Id1");
            RenameIndex(table: "dbo.PatientClinicalEvent", name: "__mig_tmp__0", newName: "IX_Medication_Id");
            CreateTable(
                "dbo.DatasetCategoryElementCondition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Condition_Id = c.Int(),
                        DatasetCategoryElement_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Condition", t => t.Condition_Id)
                .ForeignKey("dbo.DatasetCategoryElement", t => t.DatasetCategoryElement_Id)
                .Index(t => t.Condition_Id)
                .Index(t => t.DatasetCategoryElement_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DatasetCategoryElementCondition", "DatasetCategoryElement_Id", "dbo.DatasetCategoryElement");
            DropForeignKey("dbo.DatasetCategoryElementCondition", "Condition_Id", "dbo.Condition");
            DropIndex("dbo.DatasetCategoryElementCondition", new[] { "DatasetCategoryElement_Id" });
            DropIndex("dbo.DatasetCategoryElementCondition", new[] { "Condition_Id" });
            DropTable("dbo.DatasetCategoryElementCondition");
            RenameIndex(table: "dbo.PatientClinicalEvent", name: "IX_Medication_Id", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.PatientClinicalEvent", name: "IX_Medication_Id1", newName: "IX_Medication_Id");
            RenameIndex(table: "dbo.PatientClinicalEvent", name: "__mig_tmp__0", newName: "IX_Medication_Id1");
            RenameColumn(table: "dbo.PatientClinicalEvent", name: "Medication_Id", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.PatientClinicalEvent", name: "Medication_Id1", newName: "Medication_Id");
            RenameColumn(table: "dbo.PatientClinicalEvent", name: "__mig_tmp__0", newName: "Medication_Id1");
        }
    }
}
