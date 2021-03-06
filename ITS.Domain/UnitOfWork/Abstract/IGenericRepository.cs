﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.Domain.UnitOfWork.Abstract
{
	public interface IGenericRepository<TEntity>
	{
		IQueryable<TEntity> GetAll();
		TEntity GetByID(object id);
		void Insert(TEntity entity);
		void Delete(object id);
		void Delete(TEntity entityToDelete);
		void Update(TEntity entityToUpdate);

		void Save();
	}
}
