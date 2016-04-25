using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ITS.Domain.Entities;

namespace ITS.Domain
{
	public class EFDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Test> Tests { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<ABCDQuestion> ABCDQuestions { get; set; }
		public DbSet<TextQuestion> TextQuestions { get; set; }
		public DbSet<Result> Results { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<Subject> Subjects { get; set; }

		public EFDbContext() :
			base ("ITSDatabase")
		{

		}
	}
}