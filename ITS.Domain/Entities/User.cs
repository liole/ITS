using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ITS.Domain.Entities
{
	public class User
	{
        [HiddenInput(DisplayValue = false)]
		public int ID { get; set; }

		[Required(ErrorMessage = "Please enter user's First Name")]
		public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter user's First Name")]
		public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter user's Login")]
        public string Login { get; set; }
		public string Password { get; set; }
        //[Required(ErrorMessage = "Please select user's Role")]
		//public UserRole Role { get; set; }
		public bool IsStudent { get; set; }
		public bool IsTeacher { get; set; }
		public bool IsAdmin { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
	}

	public enum UserRole 
	{
		Student, Teacher, Admin
	}
}