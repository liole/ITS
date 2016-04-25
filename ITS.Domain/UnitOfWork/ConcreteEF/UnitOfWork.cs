using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Domain.UnitOfWork.Abstract;
using ITS.Domain.Entities;

namespace ITS.Domain.UnitOfWork.ConcreteEF
{
	public class UnitOfWork : IUnitOfWork
	{
		public IGenericRepository<User> Users { get; set; }
		public IGenericRepository<Test> Tests { get; set; }
		public IGenericRepository<Question> Questions { get; set; }
		public IGenericRepository<ABCDQuestion> ABCDQuestions { get; set; }
		public IGenericRepository<TextQuestion> TextQuestions { get; set; }
		public IGenericRepository<Result> Results { get; set; }
		public IGenericRepository<Group> Groups { get; set; }
		public IGenericRepository<Subject> Subjects { get; set; }

		private EFDbContext context;

		public UnitOfWork(EFDbContext context)
		{
			this.context = context;

			this.Users = new GenericRepository<User>(context);
			this.Tests = new GenericRepository<Test>(context);
			this.Questions = new GenericRepository<Question>(context);
			this.ABCDQuestions = new GenericRepository<ABCDQuestion>(context);
			this.TextQuestions = new GenericRepository<TextQuestion>(context);
			this.Results = new GenericRepository<Result>(context);
			this.Groups = new GenericRepository<Group>(context);
			this.Subjects = new GenericRepository<Subject>(context);
		}

		public void Save()
		{
			this.context.SaveChanges();
		}
	}
}