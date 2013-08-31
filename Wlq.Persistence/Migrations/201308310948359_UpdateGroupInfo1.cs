namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateGroupInfo1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupInfoes", "Introduction", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupInfoes", "Introduction");
        }
    }
}
