namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePostInfo1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostInfoes", "Phone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostInfoes", "Phone");
        }
    }
}
