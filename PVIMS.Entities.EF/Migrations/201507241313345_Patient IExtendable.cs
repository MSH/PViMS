namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatientIExtendable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patient", "CustomAttributesXmlSerialised", c => c.String(storeType: "xml"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patient", "CustomAttributesXmlSerialised");
        }
    }
}
