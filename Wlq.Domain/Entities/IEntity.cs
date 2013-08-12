using System;

namespace Wlq.Domain
{
	public interface IEntity
	{
		long Id { get; set; }

		DateTime LastModified { get; set; }
	}
}
