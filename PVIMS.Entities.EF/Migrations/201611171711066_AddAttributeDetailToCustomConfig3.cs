namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttributeDetailToCustomConfig3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomAttributeConfiguration", "AttributeDetail", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomAttributeConfiguration", "AttributeDetail");
        }
    }
}
