namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRangetoMetaColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaColumn", "Range", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaColumn", "Range");
        }
    }
}
