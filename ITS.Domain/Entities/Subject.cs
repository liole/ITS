using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ITS.Domain.Entities
{
	public class Subject
	{
        [HiddenInput(DisplayValue = false)]
		public int ID { get; set; }

        [Required(ErrorMessage = "Please enter subject's Name")]
		public string Name { get; set; }

        public virtual ICollection<Test> Tests { get; set; }
	}
}