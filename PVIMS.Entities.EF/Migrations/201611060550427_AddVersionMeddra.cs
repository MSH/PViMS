namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersionMeddra : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TerminologyMedDra", "MedDraVersion", c => c.String(maxLength: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TerminologyMedDra", "MedDraVersion");
        }
    }
}
