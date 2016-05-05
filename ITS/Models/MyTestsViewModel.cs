using ITS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Models
{
	public class MarkedTest
	{
		public Test Test { get; set; }
		public int Mark { get; set; }
	}
	public class MyTestsViewModel
	{
		public IEnumerable<Test> Present { get; set; }
		public IEnumerable<MarkedTest> Past { get; set; }
	}
}