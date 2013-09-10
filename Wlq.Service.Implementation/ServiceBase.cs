using System;
using System.Collections.Generic;

using Hanger.Common;
using Wlq.Domain;
using Wlq.Persistence;
using Wlq.Service.Utility;

namespace Wlq.Service.Implementation
{
	public abstract class ServiceBase : Disposable
	{
		private Dictionary<Type, object> _repositoryContainer = new Dictionary<Type, object>();
		protected readonly DatabaseContext _databaseContext;

		public ServiceBase(DatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		protected DatabaseRepository<TEntity> RepositoryProvider<TEntity>()
			where TEntity : class, IEntity
		{
			var type = typeof(TEntity);

			if (_repositoryContainer.ContainsKey(type))
			{
				return _repositoryContainer[type] as DatabaseRepository<TEntity>;
			}
			else
			{
				var repository = new DatabaseRepository<TEntity>(_databaseContext);

				_repositoryContainer.Add(type, repository);

				return repository;
			}
		}

		protected EntityList<TEntity> GetList<TEntity>(bool fromCache, string key, int pageIndex, int pageSize, TimeSpan cacheTime, Func<IEnumerable<TEntity>> getEntities)
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
