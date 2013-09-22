namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntities3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostInfoes", "VenueGroupId", c => c.Long(nullable: false));
            AddColumn("dbo.BookingInfoes", "InvolvedType", c => c.Int(nullable: false));
            AddColumn("dbo.BookingInfoes", "IsPresent", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserInfoes", "Code", c => c.String());
            DropColumn("dbo.PostInfoes", "VenueId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostInfoes", "VenueId", c => c.Long(nullable: false));
            DropColumn("dbo.UserInfoes", "Code");
            DropColumn("dbo.BookingInfoes", "IsPresent");
            DropColumn("dbo.BookingInfoes", "InvolvedType");
            DropColumn("dbo.PostInfoes", "VenueGroupId");
        }
    }
}
