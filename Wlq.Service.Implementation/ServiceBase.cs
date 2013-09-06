﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hanger.Common;
using Wlq.Persistence;
using Wlq.Domain;

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
	}
}