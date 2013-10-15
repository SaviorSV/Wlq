namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateVenueGroupInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VenueGroupInfoes", "PostType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VenueGroupInfoes", "PostType");
        }
    }
}
