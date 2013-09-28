namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveColumnFromMessageInfo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MessageInfoes", "PostType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MessageInfoes", "PostType", c => c.Int(nullable: false));
        }
    }
}
