namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDownloadDatasettoUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "AllowDatasetDownload", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "AllowDatasetDownload");
        }
    }
}
