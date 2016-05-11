using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork;
using ITS.Models;
using System.Web.Helpers;

namespace ITS.Controllers
{
    public class AccountController : Controller
    {
        private IUnitOfWork unitOfWork;

        public AccountController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public ActionResult Profile()
        {
            var user = CurrentUser();
            var profile = new UserProfilesViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Login = user.Login
            };

            return View(profile);
        }

        [HttpPost]
        public ActionResult Profile(UserProfilesViewModel profile)
        {
            var user = CurrentUser();
            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            if (profile.OldPassword != null)
            {
                if (Crypto.VerifyHashedPassword(user.Password, profile.OldPassword))
                {
                    if (profile.NewPassword != null)
                    {
                        if (profile.NewPassword == profile.ConfirmPassword)
                        {
                            user.Password = Crypto.HashPassword(profile.NewPassword);
                        }
                    }
                }
                else
                {
                    TempData["message"] = "Old password is incorrect!";
                }
            }

            unitOfWork.Users.Update(user);
            unitOfWork.Users.Save();
            return RedirectToAction("Profile", "Account");
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
			if (user.IsStudent)
			{
				return RedirectToAction("Assigned", "Test");
			}
			if (user.IsTeacher)
			{
				return RedirectToAction("List", "Test");
			}
			if (user.IsAdmin)
			{
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
