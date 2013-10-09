namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntities6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostInfoes", "IsLongterm", c => c.Boolean(nullable: false));
            AddColumn("dbo.PostInfoes", "IsAudited", c => c.Boolean(nullable: false));
            AddColumn("dbo.VenueGroupInfoes", "Phone", c => c.String());
            AddColumn("dbo.VenueGroupInfoes", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VenueGroupInfoes", "Address");
            DropColumn("dbo.VenueGroupInfoes", "Phone");
            DropColumn("dbo.PostInfoes", "IsAudited");
            DropColumn("dbo.PostInfoes", "IsLongterm");
        }
    }
}
