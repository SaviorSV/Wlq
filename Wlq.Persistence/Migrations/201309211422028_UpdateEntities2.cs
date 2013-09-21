namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntities2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VenueGroupInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        GroupId = c.Long(nullable: false),
                        VenueType = c.Int(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.VenueInfoes", "VenueGroupId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VenueInfoes", "VenueGroupId");
            DropTable("dbo.VenueGroupInfoes");
        }
    }
}
