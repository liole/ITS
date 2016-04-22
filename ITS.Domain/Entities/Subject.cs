using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Domain
{
	public class Subject
	{
		public int ID { get; set; }

		[Required]
		public string Name { get; set; }

        public virtual ICollection<Test> Tests { get; set; }
	}
}