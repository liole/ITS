using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Domain.Entities
{
	public class Group
	{
		public int ID { get; set; }

		[Required]
		public string Name { get; set; }

		public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
	}
}