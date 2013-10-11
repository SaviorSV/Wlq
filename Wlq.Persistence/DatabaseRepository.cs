using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Hanger.Common;
using Wlq.Domain;

namespace Wlq.Persistence
{
	public class DatabaseRepository<TEntity>
		where TEntity : EntityBase
	{
		private readonly DatabaseContext _context;
		private readonly DbSet<TEntity> _dbSet;

		public DatabaseRepository(DatabaseContext context)
		{
			_context = context;
			_dbSet = _context.Set<TEntity>();
		}

		public IQueryable<TEntity> Entities
		{
			get
			{
				return _dbSet;
			}
		}

		public TEntity GetById(long id)
		{
			return _dbSet.FirstOrDefault(o => o.Id == id);
		}

		public int Add(TEntity entity, bool isSave)
		{
			entity.LastModified = DateTime.Now;

			_dbSet.Add(entity);

			return isSave ? _context.SaveChanges() : 0;
		}

		public bool BatchInsert(IList<TEntity> entities)
		{
			try
			{
				_context.Configuration.AutoDetectChangesEnabled = false;

				var count = 0;

				foreach (var entityToInsert in entities)
				{
					++count;
					AddToContext(entityToInsert, count, 100);
				}

				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				LocalLoggingService.Exception(ex);

				return false;
			}

			return true;
		}

		private void AddToContext(TEntity entity, int count, int commitCount)
		{
			this.Add(entity, false);

			if (count % commitCount == 0)
			{
				_context.SaveChanges();
			}
		}

		public int Update(TEntity entity, bool isSave)
		{
			entity.LastModified = DateTime.Now;

			return isSave ? _context.SaveChanges() : 0;
		}

		public int DeleteById(long id, bool isSave)
		{
			var entity = this.GetById(id);

			if (entity != null)
				_dbSet.Remove(entity);

			return isSave ? _context.SaveChanges() : 0;
		}

		public int Delete(TEntity entity, bool isSave)
		{
			if (entity != null)
				_dbSet.Remove(entity);

			return isSave ? _context.SaveChanges() : 0;
		}
	}
}
