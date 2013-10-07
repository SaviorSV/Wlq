namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePostInfo4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostInfoes", "PhoneBookingNumber", c => c.String());
            AddColumn("dbo.PostInfoes", "PhoneBookingTime", c => c.String());
            AddColumn("dbo.PostInfoes", "SpotBookingNumber", c => c.String());
            AddColumn("dbo.PostInfoes", "SpotBookingAddress", c => c.String());
            AddColumn("dbo.PostInfoes", "SpotBookingTime", c => c.String());
            DropColumn("dbo.PostInfoes", "Location");
            DropColumn("dbo.PostInfoes", "RelatedPlace");
            DropColumn("dbo.PostInfoes", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostInfoes", "Phone", c => c.String());
            AddColumn("dbo.PostInfoes", "RelatedPlace", c => c.String());
            AddColumn("dbo.PostInfoes", "Location", c => c.String());
            DropColumn("dbo.PostInfoes", "SpotBookingTime");
            DropColumn("dbo.PostInfoes", "SpotBookingAddress");
            DropColumn("dbo.PostInfoes", "SpotBookingNumber");
            DropColumn("dbo.PostInfoes", "PhoneBookingTime");
            DropColumn("dbo.PostInfoes", "PhoneBookingNumber");
        }
    }
}
