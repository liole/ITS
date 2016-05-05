using ITS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Models
{
	public class GroupTestViewModel
	{
		public Group Group { get; set; }
		public bool IsAssigned { get; set; }
	}
}