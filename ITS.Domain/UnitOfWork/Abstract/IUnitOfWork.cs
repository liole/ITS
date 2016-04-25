using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.Domain.Entities;

namespace ITS.Domain.UnitOfWork.Abstract
{
	public interface IUnitOfWork
	{
		IGenericRepository<User> Users { get; set; }
		IGenericRepository<Test> Tests { get; set; }
		IGenericRepository<Question> Questions { get; set; }
		IGenericRepository<ABCDQuestion> ABCDQuestions { get; set; }
		IGenericRepository<TextQuestion> TextQuestions { get; set; }
		IGenericRepository<Result> Results { get; set; }
		IGenericRepository<Group> Groups { get; set; }
		IGenericRepository<Subject> Subjects { get; set; }

		void Save();
	}
}
