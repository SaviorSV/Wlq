using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

using Wlq.Domain;

namespace Wlq.Persistence
{
	public class DatabaseContext : DbContext
	{
		public DbSet<GroupInfo> Groups { get; set; }
		public DbSet<PostInfo> Posts { get; set; }
		public DbSet<VenueInfo> Venues { get; set; }
		public DbSet<UserInfo> Users { get; set; }
		public DbSet<UserGroupInfo> UserGroups { get; set; }
		public DbSet<UserPostInfo> UserPosts { get; set; }
	}
}
