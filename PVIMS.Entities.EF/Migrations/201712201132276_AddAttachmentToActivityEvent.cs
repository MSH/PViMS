namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentToActivityEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachment", "ActivityExecutionStatusEvent_Id", c => c.Int());
            CreateIndex("dbo.Attachment", "ActivityExecutionStatusEvent_Id");
            AddForeignKey("dbo.Attachment", "ActivityExecutionStatusEvent_Id", "dbo.ActivityExecutionStatusEvent", "Id");
            DropColumn("dbo.Attachment", "CustomAttributesXmlSerialised");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attachment", "CustomAttributesXmlSerialised", c => c.String(storeType: "xml"));
            DropForeignKey("dbo.Attachment", "ActivityExecutionStatusEvent_Id", "dbo.ActivityExecutionStatusEvent");
            DropIndex("dbo.Attachment", new[] { "ActivityExecutionStatusEvent_Id" });
            DropColumn("dbo.Attachment", "ActivityExecutionStatusEvent_Id");
        }
    }
}
