using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Domain.Entities
{
	public class Result
	{
		public int ID { get; set; }

		[Required]
		public int TestID { get; set; }
		[Required]
		public int UserID { get; set; }
		[Required]
		public int Mark { get; set; }
		[Required]
		public int QuestionAmountCorrect { get; set; }

		public virtual User User { get; set; }
		public virtual Test Test { get; set; }
	}
}