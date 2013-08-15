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

		public IQueryable<TEntity> GetAll()
		{
			return _dbSet;
		}

		public void Add(TEntity entity)
		{
			entity.LastModified = DateTime.Now;

			_dbSet.Add(entity);
		}

		public TEntity GetById(long id)
		{
			return _dbSet.FirstOrDefault(o => o.Id == id);
		}

		public void DeleteById(long id)
		{
			var entity = this.GetById(id);

			if (entity != null)
				_dbSet.Remove(entity);
		}

		public void Update(TEntity entity)
		{
			entity.LastModified = DateTime.Now;
		}
	}
}
