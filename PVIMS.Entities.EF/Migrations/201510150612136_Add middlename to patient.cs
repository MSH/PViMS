namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmiddlenametopatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patient", "MiddleName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patient", "MiddleName");
        }
    }
}
