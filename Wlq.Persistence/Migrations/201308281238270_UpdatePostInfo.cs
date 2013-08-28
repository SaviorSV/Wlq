namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePostInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostInfoes", "Publisher", c => c.String());
            AddColumn("dbo.PostInfoes", "PostType", c => c.Int(nullable: false));
            DropColumn("dbo.PostInfoes", "GroupType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostInfoes", "GroupType", c => c.Int(nullable: false));
            DropColumn("dbo.PostInfoes", "PostType");
            DropColumn("dbo.PostInfoes", "Publisher");
        }
    }
}
