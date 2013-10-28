namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserInfo2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "CreateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.UserInfoes", "CardNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserInfoes", "CardNumber", c => c.String());
            DropColumn("dbo.UserInfoes", "CreateTime");
        }
    }
}
