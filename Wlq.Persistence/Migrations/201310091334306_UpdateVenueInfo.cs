namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateVenueInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VenueInfoes", "Phone", c => c.String());
            AddColumn("dbo.VenueInfoes", "IsSuspend", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VenueInfoes", "IsSuspend");
            DropColumn("dbo.VenueInfoes", "Phone");
        }
    }
}
