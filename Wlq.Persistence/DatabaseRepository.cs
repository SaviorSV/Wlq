﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wlq.Domain;
using System.Data.Entity;

namespace Wlq.Persistence
{
	public class DatabaseRepository<TEntity>
		where TEntity : class, IEntity
	{
		private readonly DatabaseContext _context;
		private readonly DbSet<TEntity> _dbSet;

		public DatabaseRepository(DatabaseContext context)
		{
			_context = context;
			_dbSet = _context.Set<TEntity>();
		}

		public IQueryable<TEntity> GetAll()
		{
			return _dbSet;
		}

		public void Add(TEntity entity)
		{
			_dbSet.Add(entity);
		}

		public TEntity Get(long id)
		{
			return _dbSet.FirstOrDefault(o => o.Id == id);
		}

		public void Delete(TEntity entity)
		{
			_dbSet.Remove(entity);
		}
	}
}
