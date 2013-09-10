using System.Collections.Generic;

namespace Wlq.Domain
{
	public class EntityList<TEntity>
	{
		public int TotalNumber { get; set; }
		public IEnumerable<TEntity> List { get; set; }
	}
}
