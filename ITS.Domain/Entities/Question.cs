using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Domain.Entities
{
	public abstract class Question
	{
		public int ID { get; set; }

		[Required]
		public string QuestionText { get; set; }
		[Required]
		public decimal Coefficient { get; set; }
		[Required]
		public int TestID { get; set; }

		public virtual Test Test { get; set; }
	}
}