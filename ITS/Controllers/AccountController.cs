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
                            user.Password = Crypto.Hash(profile.NewPassword);
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

        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Account/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Account/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Account/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Account/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Account/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private User CurrentUser()
        {
            return unitOfWork.Users.GetByID(1);
        }
    }
}
