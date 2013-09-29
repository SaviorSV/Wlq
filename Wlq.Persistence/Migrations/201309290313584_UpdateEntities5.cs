namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntities5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostInfoes", "BookingTypes", c => c.Int(nullable: false));
            AddColumn("dbo.PostInfoes", "InvolvedTypes", c => c.Int(nullable: false));
            AddColumn("dbo.BookingInfoes", "VenueId", c => c.Long(nullable: false));
            AddColumn("dbo.BookingInfoes", "BookingType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookingInfoes", "BookingType");
            DropColumn("dbo.BookingInfoes", "VenueId");
            DropColumn("dbo.PostInfoes", "InvolvedTypes");
            DropColumn("dbo.PostInfoes", "BookingTypes");
        }
    }
}
