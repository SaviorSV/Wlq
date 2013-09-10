using System;
using System.Collections.Generic;

using Hanger.Common;
using Wlq.Domain;
using Wlq.Service.Utility;

namespace Wlq.Service.Implementation
{
	public class CacheService
	{
		public static EntityList<TEntity> GetList<TEntity>(bool fromCache, string key, int pageIndex, int pageSize, TimeSpan cacheTime, Func<IEnumerable<TEntity>> getEntities)
		{
			var totalNumber = 0;
			var entityList = fromCache ? CacheHelper<EntityList<TEntity>>.Get(key) : null;

			if (entityList == null)
			{
				var entities = getEntities().Paging(pageIndex, pageSize, out totalNumber);

				entityList = new EntityList<TEntity> { TotalNumber = totalNumber, List = entities };

				if (fromCache)
				{
					CacheHelper<EntityList<TEntity>>.Set(key, entityList, cacheTime);
				}
			}

			return entityList;
		}
	}
}
