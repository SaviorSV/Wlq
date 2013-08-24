namespace Wlq.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Logo = c.String(),
                        ParentGroupId = c.Long(nullable: false),
                        GroupType = c.Int(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupManagerInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        GroupId = c.Long(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        Image = c.String(),
                        GroupId = c.Long(nullable: false),
                        GroupType = c.Int(nullable: false),
                        BeginDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        VenueId = c.Long(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VenueInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        GroupId = c.Long(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VenueConfigInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        VenueId = c.Long(nullable: false),
                        DaysOfWeek = c.Int(nullable: false),
                        BegenTime = c.Int(nullable: false),
                        EndTime = c.Int(nullable: false),
                        LimitNumber = c.Int(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LoginName = c.String(),
                        Password = c.String(),
                        Name = c.String(),
                        Gender = c.String(),
                        Birth = c.String(),
                        Mobile = c.String(),
                        Committees = c.String(),
                        Address = c.String(),
                        Tags = c.Int(nullable: false),
                        Role = c.Int(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserGroupInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        GroupId = c.Long(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPostInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        PostId = c.Long(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserPostInfoes");
            DropTable("dbo.UserGroupInfoes");
            DropTable("dbo.UserInfoes");
            DropTable("dbo.VenueBookingInfoes");
            DropTable("dbo.VenueConfigInfoes");
            DropTable("dbo.VenueInfoes");
            DropTable("dbo.PostInfoes");
            DropTable("dbo.GroupManagerInfoes");
            DropTable("dbo.GroupInfoes");
        }
    }
}
