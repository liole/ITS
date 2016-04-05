using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Models
{
	public class Test
	{
		public int ID { get; set; }

		[Required]
		public string Name { get; set; }
		[Required]
		public int SubjectID { get; set; }
		[Required]
		public bool Randomize { get; set; }
		[Required]
		public int Mark { get; set; }

		public virtual Subject Subject { get; set; }
		public virtual IEnumerable<Question> Questions { get; set; }
		public virtual IEnumerable<Group> Groups { get; set; }
		public virtual IEnumerable<Result> Results { get; set; }
	}
}