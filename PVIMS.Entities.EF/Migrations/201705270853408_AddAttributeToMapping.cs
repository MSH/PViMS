namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttributeToMapping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetMapping", "Attribute", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetMapping", "Attribute");
        }
    }
}
