namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateMessageInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MessageInfoes", "PostId", c => c.Long(nullable: false));
            AddColumn("dbo.MessageInfoes", "PostType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MessageInfoes", "PostType");
            DropColumn("dbo.MessageInfoes", "PostId");
        }
    }
}
