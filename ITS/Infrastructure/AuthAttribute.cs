using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ITS.Infrastructure
{
	public class AuthAttribute : ActionFilterAttribute
	{
		private UserRole[] roles;

		public AuthAttribute()
		{
			this.roles = null;
		}
		public AuthAttribute(params UserRole[] roles)
		{
			this.roles = roles;
		}

		private User CurrentUser(ActionExecutingContext filterContext)
		{
			var id = filterContext.RequestContext.HttpContext.Session["user"];
			if (id == null)
			{
				return null;
			}
			var unitOfWork = System.Web.Mvc.DependencyResolver.Current.GetService<IUnitOfWork>();
			return unitOfWork.Users.GetByID((int)id);
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var user = CurrentUser(filterContext);
			if (user == null)
			{
				filterContext.Result = new RedirectResult("/Account/Login");
				return;
			}
			if (roles == null)
			{
				return;
			}
			var allow = 
				(user.IsAdmin && roles.Contains(UserRole.Admin)) ||
				(user.IsTeacher && roles.Contains(UserRole.Teacher)) ||
				(user.IsStudent && roles.Contains(UserRole.Student));
			if (!allow)
			{
				filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
				return;
			}
		}
	}
}