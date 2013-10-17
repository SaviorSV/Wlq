namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateBookingInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookingInfoes", "PresentTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookingInfoes", "PresentTime");
        }
    }
}
