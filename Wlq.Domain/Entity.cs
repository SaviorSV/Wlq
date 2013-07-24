using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wlq.Domain
{
	public abstract class Entity : IEntity
	{
		public long Id { get; set; }

		public DateTime LastModified { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Entity))
				return false;

			if (Object.ReferenceEquals(this, obj))
				return true;

			var item = (Entity)obj;

			return item.Id == this.Id;
		}

		private int? _requestedHashCode;

		public override int GetHashCode()
		{
			if (!_requestedHashCode.HasValue)
				_requestedHashCode = this.Id.GetHashCode() ^ 31;

			return _requestedHashCode.Value;
		}

		public static bool operator ==(Entity left, Entity right)
		{
			if (Object.Equals(left, null))
				return (Object.Equals(right, null)) ? true : false;
			else
				return left.Equals(right);
		}

		public static bool operator !=(Entity left, Entity right)
		{
			return !(left == right);
		}
	}
}
