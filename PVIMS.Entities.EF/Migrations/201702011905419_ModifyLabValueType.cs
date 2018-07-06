namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyLabValueType : DbMigration
    {
        public override void Up()
        {
            Sql(DropDefaultConstraint("dbo.PatientLabTest", "LabValue"));
            AlterColumn("dbo.PatientLabTest", "LabValue", c => c.String(maxLength: 20));
            AlterColumn("dbo.PatientLabTest", "ReferenceLower", c => c.String(maxLength: 20));
            AlterColumn("dbo.PatientLabTest", "ReferenceUpper", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientLabTest", "ReferenceUpper", c => c.String(maxLength: 100));
            AlterColumn("dbo.PatientLabTest", "ReferenceLower", c => c.String(maxLength: 100));
            AlterColumn("dbo.PatientLabTest", "LabValue", c => c.Decimal(precision: 18, scale: 2));
        }

        private string DropDefaultConstraint(string table, string column)
        {
            return string.Format(@"
            DECLARE @name sysname

            SELECT @name = dc.name
            FROM sys.columns c
            JOIN sys.default_constraints dc ON dc.object_id = c.default_object_id
            WHERE c.object_id = OBJECT_ID('{0}')
            AND c.name = '{1}'

            IF @name IS NOT NULL
                EXECUTE ('ALTER TABLE {0} DROP CONSTRAINT ' + @name)
            ",
                table, column);
        }
    }

}
