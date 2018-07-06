namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumnNameSizeOnMetaColumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MetaColumn", "ColumnName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MetaColumn", "ColumnName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
