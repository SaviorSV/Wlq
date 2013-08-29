namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookingInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        PostId = c.Long(nullable: false),
                        Name = c.String(),
                        Mobile = c.String(),
                        VenueConfigId = c.Long(nullable: false),
                        BookingDate = c.DateTime(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PostInfoes", "Location", c => c.String());
            AddColumn("dbo.PostInfoes", "RelatedPlace", c => c.String());
            AddColumn("dbo.PostInfoes", "Fee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PostInfoes", "LimitNumber", c => c.Int(nullable: false));
            AddColumn("dbo.PostInfoes", "BookingNumber", c => c.Int(nullable: false));
            DropTable("dbo.VenueBookingInfoes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.VenueBookingInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        VenueId = c.Long(nullable: false),
                        VenueConfigId = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        PostId = c.Long(nullable: false),
                        BookingDate = c.DateTime(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.PostInfoes", "BookingNumber");
            DropColumn("dbo.PostInfoes", "LimitNumber");
            DropColumn("dbo.PostInfoes", "Fee");
            DropColumn("dbo.PostInfoes", "RelatedPlace");
            DropColumn("dbo.PostInfoes", "Location");
            DropTable("dbo.BookingInfoes");
        }
    }
}
