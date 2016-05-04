using ITS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Models
{
	public class TestResultViewModel
	{
		public Test Test { get; set; }
		public int Count { get; set; }
		public int Result { get; set; }
	}
}