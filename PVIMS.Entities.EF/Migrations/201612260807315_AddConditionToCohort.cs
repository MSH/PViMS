namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConditionToCohort : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CohortGroup", "Condition_Id", c => c.Int());
            CreateIndex("dbo.CohortGroup", "Condition_Id");
            AddForeignKey("dbo.CohortGroup", "Condition_Id", "dbo.Condition", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CohortGroup", "Condition_Id", "dbo.Condition");
            DropIndex("dbo.CohortGroup", new[] { "Condition_Id" });
            DropColumn("dbo.CohortGroup", "Condition_Id");
        }
    }
}
