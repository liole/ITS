using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Models
{
	public class QuestionAnswerModel
	{
		public int ID { get; set; }
		public string Answer { get; set; }
	}
	public class TestAnswersModel
	{
		public int TestID { get; set; }
		public IEnumerable<QuestionAnswerModel> Questions { get; set; }
	}
}