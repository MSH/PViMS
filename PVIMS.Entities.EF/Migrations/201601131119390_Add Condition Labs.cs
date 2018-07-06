namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConditionLabs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConditionLabTest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Condition_Id = c.Int(),
                        LabTest_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Condition", t => t.Condition_Id)
                .ForeignKey("dbo.LabTest", t => t.LabTest_Id)
                .Index(t => t.Condition_Id)
                .Index(t => t.LabTest_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConditionLabTest", "LabTest_Id", "dbo.LabTest");
            DropForeignKey("dbo.ConditionLabTest", "Condition_Id", "dbo.Condition");
            DropIndex("dbo.ConditionLabTest", new[] { "LabTest_Id" });
            DropIndex("dbo.ConditionLabTest", new[] { "Condition_Id" });
            DropTable("dbo.ConditionLabTest");
        }
    }
}
