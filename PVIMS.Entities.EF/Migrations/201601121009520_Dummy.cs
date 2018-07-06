namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dummy : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DatasetElement", "ElementName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Medication", "DrugName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Medication", "DrugName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.DatasetElement", "ElementName", c => c.String(maxLength: 50));
        }
    }
}
