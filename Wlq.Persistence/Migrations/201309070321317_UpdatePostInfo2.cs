namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePostInfo2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostInfoes", "IsHealthTopic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostInfoes", "IsHealthTopic");
        }
    }
}
