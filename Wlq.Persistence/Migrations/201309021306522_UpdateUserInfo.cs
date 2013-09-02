namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "Avatar", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoes", "Avatar");
        }
    }
}
