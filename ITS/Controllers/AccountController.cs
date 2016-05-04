using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ITS.Controllers
{
    public class AccountController : Controller
    {
        private IUnitOfWork unitOfWork;

		public AccountController(IUnitOfWork uow)
		{
			this.unitOfWork = uow;
		}

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(string login, string password)
		{
			var user = unitOfWork.Users.GetAll().FirstOrDefault(u => u.Login == login);
			if (user == null)
			{
				TempData["message"] = string.Format("Не правильне ім'я користувача");
				return View();
			}
			if (!Crypto.VerifyHashedPassword(user.Password, password))
			{
				TempData["message"] = string.Format("Не правильний пароль");
				return View();
			}
			Session.Add("user", user.ID);
			return Redirect();
		}

		public ActionResult Logout()
		{
			Session.Remove("user");
			return Redirect();
		}

		public ActionResult Redirect()
		{
			var user = CurrentUser();
			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}
			switch (user.Role)
			{
				case UserRole.Student:
					return RedirectToAction("Assigned", "Test");
				case UserRole.Teacher:
					return RedirectToAction("List", "Test");
				case UserRole.Admin:
					return RedirectToAction("List", "User");
			}
			return RedirectToAction("Login", "Account");
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
