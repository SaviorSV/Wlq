namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePostInfo5 : DbMigration
    {
        public override void Up()
        {
			AddColumn("dbo.PostInfoes", "Phone", c => c.String());
			AddColumn("dbo.PostInfoes", "Address", c => c.String());
        }
        
        public override void Down()
        {
			DropColumn("dbo.PostInfoes", "Phone");
			DropColumn("dbo.PostInfoes", "Address");
        }
    }
}
