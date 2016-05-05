using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork.Abstract;
using ITS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Controllers
{
    public class NavigationController : Controller
    {
		private IUnitOfWork unitOfWork;

		public NavigationController(IUnitOfWork uow)
		{
			this.unitOfWork = uow;
		}

		public PartialViewResult Menu()
		{
			var user = CurrentUser();
			if (user == null)
			{
				return PartialView(null);
			}
			var menuList = new List<LinkViewModel>();
			string controller = ControllerContext.ParentActionViewContext.RouteData.Values["controller"].ToString();
			string action = ControllerContext.ParentActionViewContext.RouteData.Values["action"].ToString();

			menuList.Add(new LinkViewModel("Мої тести", "Assigned", "Test", controller == "Test" && (action == "Assigned" || user.Role == UserRole.Student)));
			menuList.Add(null); //separator
			if (user.Role != UserRole.Student)
			{
				menuList.Add(new LinkViewModel("Тести", "List", "Test", controller == "Test" && action != "Assigned"));
				menuList.Add(new LinkViewModel("Користувачі", "List", "User", controller == "User"));
				menuList.Add(new LinkViewModel("Групи", "List", "Group", controller == "Group"));
				menuList.Add(new LinkViewModel("Предмети", "List", "Subject", controller == "Subject"));
				menuList.Add(null);
			}
			menuList.Add(new LinkViewModel("Профіль", "Profile", "Account", controller == "Account"));
			menuList.Add(new LinkViewModel("Вихід", "Logout", "Account", false));
			return PartialView(menuList);
		}

		private User CurrentUser()
		{
			var id = Session["user"];
			if (id == null)
			{
				return null;
			}
			return unitOfWork.Users.GetByID((int)id);
		}

    }
}
