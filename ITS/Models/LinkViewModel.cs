using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Models
{
	public class LinkViewModel
	{
		public string Text { get; set; }
		public string Action { get; set; }
		public string Controller { get; set; }
		public bool Selected { get; set; }

		public LinkViewModel(string text, string Action, string controller, bool selected = false)
		{
			this.Text = text;
			this.Action = Action;
			this.Controller = controller;
			this.Selected = selected;
		}
	}
}