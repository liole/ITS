using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITS.Models
{
	public class User
	{
		public int ID { get; set; }

		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		public string Login { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public UserRole Role { get; set; }

		public virtual IEnumerable<Group> Groups { get; set; }
		public virtual IEnumerable<Result> Results { get; set; }
	}

	public enum UserRole 
	{
		Student, Teacher, Admin
	}
}