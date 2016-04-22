using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Domain
{
	public class ABCDQuestion : Question
	{
		[Required]
		public string AnswerA { get; set; }
		public string AnswerB { get; set; }
		public string AnswerC { get; set; }
		public string AnswerD { get; set; }

		[Required]
		public ABCDAnswer Answer { get; set; }
	}

	public enum ABCDAnswer
	{
		A, B, C, D
	}
}