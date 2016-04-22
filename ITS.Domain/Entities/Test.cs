using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Domain
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
        [Required]
        public int UserID { get; set; }

		public virtual Subject Subject { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Result> Results { get; set; }
	}
}