using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ITS.Models.UnitOfWork.ConcreteEF
{
	public class GenericRepository<TEntity> : Abstract.IGenericRepository<TEntity> where TEntity : class 
	{
		internal EFDbContext context;
		internal DbSet<TEntity> dbSet;

		public GenericRepository(EFDbContext context)
		{
			this.context = context;
			this.dbSet = context.Set<TEntity>();
		}

		public IQueryable<TEntity> GetAll()
		{
			return this.dbSet;
		}

		public TEntity GetByID(object id)
		{
			return dbSet.Find(id);
		}

		public void Insert(TEntity entity)
		{
			dbSet.Add(entity);
		}

		public void Delete(object id)
		{
			TEntity entityToDelete = dbSet.Find(id);
			Delete(entityToDelete);
		}

		public void Delete(TEntity entityToDelete)
		{
			if (context.Entry(entityToDelete).State == EntityState.Detached)
			{
				dbSet.Attach(entityToDelete);
			}
			dbSet.Remove(entityToDelete);
		}

		public void Update(TEntity entityToUpdate)
		{
			dbSet.Attach(entityToUpdate);
			context.Entry(entityToUpdate).State = EntityState.Modified;
		}

		public void Save()
		{
			context.SaveChanges();
		}
	}
}