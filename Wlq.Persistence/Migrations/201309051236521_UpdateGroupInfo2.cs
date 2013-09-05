namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateGroupInfo2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupInfoes", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupInfoes", "Email");
        }
    }
}
