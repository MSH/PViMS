namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMappingType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DatasetMapping", "MappingType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DatasetMapping", "MappingType", c => c.String());
        }
    }
}
