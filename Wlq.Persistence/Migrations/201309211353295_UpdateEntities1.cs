namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntities1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupInfoes", "IsHealth", c => c.Boolean(nullable: false));
            AddColumn("dbo.PostInfoes", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostInfoes", "Remark");
            DropColumn("dbo.GroupInfoes", "IsHealth");
        }
    }
}
