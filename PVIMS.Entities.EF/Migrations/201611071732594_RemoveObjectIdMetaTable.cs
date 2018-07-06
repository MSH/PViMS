namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveObjectIdMetaTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MetaTable", "object_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MetaTable", "object_id", c => c.Int(nullable: false));
        }
    }
}
