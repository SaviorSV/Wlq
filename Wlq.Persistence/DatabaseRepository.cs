using System;
using System.Data.Entity;
using System.Linq;

using Wlq.Domain;

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
