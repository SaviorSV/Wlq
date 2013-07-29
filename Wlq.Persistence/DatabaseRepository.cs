using System;
using System.Data.Entity;
using System.Linq;

using Wlq.Domain;

namespace Wlq.Persistence.Implementation
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
			entity.LastModified = DateTime.Now;

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

		public void Update(TEntity entity)
		{
			entity.LastModified = DateTime.Now;
		}
	}
}
