using System.Data.Entity;

using Wlq.Domain;

namespace Wlq.Persistence
{
	public class DatabaseContext : DbContext
	{
		public DbSet<GroupInfo> Groups { get; set; }
		public DbSet<GroupManagerInfo> GroupManagers { get; set; }
		public DbSet<PostInfo> Posts { get; set; }
		public DbSet<VenueGroupInfo> Venues { get; set; }
		public DbSet<VenueInfo> VenueGroups { get; set; }
		public DbSet<VenueConfigInfo> VenueConfigs { get; set; }
		public DbSet<BookingInfo> Bookings { get; set; }
		public DbSet<UserInfo> Users { get; set; }
		public DbSet<UserGroupInfo> UserGroups { get; set; }
		public DbSet<UserPostInfo> UserPosts { get; set; }
	}
}
