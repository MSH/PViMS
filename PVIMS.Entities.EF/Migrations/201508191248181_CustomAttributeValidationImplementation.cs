namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomAttributeValidationImplementation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomAttributeConfiguration", "IsRequired", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.CustomAttributeConfiguration", "StringMaxLength", c => c.Int());
            AddColumn("dbo.CustomAttributeConfiguration", "NumericMinValue", c => c.Int());
            AddColumn("dbo.CustomAttributeConfiguration", "NumericMaxValue", c => c.Int());
            AddColumn("dbo.CustomAttributeConfiguration", "FutureDateOnly", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.CustomAttributeConfiguration", "PastDateOnly", c => c.Boolean(nullable: false, defaultValueSql: "0"));

            Sql("Update dbo.CustomAttributeConfiguration SET [IsRequired] = 0");
            Sql("Update dbo.CustomAttributeConfiguration SET [FutureDateOnly] = 0");
            Sql("Update dbo.CustomAttributeConfiguration SET [PastDateOnly] = 0");
        }

        public override void Down()
        {
            DropColumn("dbo.CustomAttributeConfiguration", "PastDateOnly");
            DropColumn("dbo.CustomAttributeConfiguration", "FutureDateOnly");
            DropColumn("dbo.CustomAttributeConfiguration", "NumericMaxValue");
            DropColumn("dbo.CustomAttributeConfiguration", "NumericMinValue");
            DropColumn("dbo.CustomAttributeConfiguration", "StringMaxLength");
            DropColumn("dbo.CustomAttributeConfiguration", "IsRequired");
        }
    }
}
