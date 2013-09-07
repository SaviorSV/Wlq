namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePostInfo3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostInfoes", "PublishTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostInfoes", "PublishTime");
        }
    }
}
