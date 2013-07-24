using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wlq.Domain
{
	public interface IEntity
	{
		long Id { get; set; }

		DateTime LastModified { get; set; }
	}
}
