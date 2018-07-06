namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatasetInstanceTagField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DatasetInstance", "Tag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DatasetInstance", "Tag");
        }
    }
}
