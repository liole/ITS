using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.Domain.Entities
{
	public class NumberQuestion : Question
	{
		[Required]
		public decimal Answer { get; set; }
	}
}
