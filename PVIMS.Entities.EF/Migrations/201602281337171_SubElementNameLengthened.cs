namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubElementNameLengthened : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DatasetElementSub", "ElementName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DatasetElementSub", "ElementName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
