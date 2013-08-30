namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateGroupInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupInfoes", "Address", c => c.String());
            AddColumn("dbo.GroupInfoes", "Phone", c => c.String());
            AddColumn("dbo.GroupInfoes", "WorkTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupInfoes", "WorkTime");
            DropColumn("dbo.GroupInfoes", "Phone");
            DropColumn("dbo.GroupInfoes", "Address");
        }
    }
}
