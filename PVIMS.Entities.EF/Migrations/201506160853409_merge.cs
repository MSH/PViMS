namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class merge : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CustomAttributeConfiguration");
            DropPrimaryKey("dbo.SelectionDataItem");
            AlterColumn("dbo.CustomAttributeConfiguration", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.SelectionDataItem", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.CustomAttributeConfiguration", "Id");
            AddPrimaryKey("dbo.SelectionDataItem", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SelectionDataItem");
            DropPrimaryKey("dbo.CustomAttributeConfiguration");
            AlterColumn("dbo.SelectionDataItem", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.CustomAttributeConfiguration", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.SelectionDataItem", "Id");
            AddPrimaryKey("dbo.CustomAttributeConfiguration", "Id");
        }
    }
}
