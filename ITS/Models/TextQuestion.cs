using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Models
{
	public class TextQuestion : Question
	{
		[Required]
		public string Answer { get; set; }
	}
}