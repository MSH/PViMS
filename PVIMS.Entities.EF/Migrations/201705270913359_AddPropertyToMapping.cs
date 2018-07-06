namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertyToMapping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetMapping", "PropertyPath", c => c.String());
            AddColumn("dbo.DatasetMapping", "Property", c => c.String());
            DropColumn("dbo.DatasetMapping", "Attribute");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DatasetMapping", "Attribute", c => c.String());
            DropColumn("dbo.DatasetMapping", "Property");
            DropColumn("dbo.DatasetMapping", "PropertyPath");
        }
    }
}
