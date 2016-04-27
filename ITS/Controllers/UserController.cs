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
    public class UserController : Controller
    {
        private IUnitOfWork unitOfWork;

        public int PageSize = 10;

        public UserController(IUnitOfWork uow)
        {
            this.unitOfWork = uow;
        }

        //
        // GET: /User/

        public ViewResult List(int page = 1)
        {
            UsersListViewModel model = new UsersListViewModel
            {
                Users = unitOfWork.Users.GetAll()
                .OrderBy(o => o.LastName).ToList()
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = unitOfWork.Users.GetAll().Count()
                }
            };

            return View(model);
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /User/Create

        public ViewResult Create()
        {
            return View("Edit",new User());
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                unitOfWork.Users.Insert(user);
                unitOfWork.Save();

                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.Error = true;
                return View(user);
            }
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            User user = unitOfWork.Users.GetByID(id);

            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, User user)
        {
            if(ModelState.IsValid)
            {
                SaveUser(user);
                TempData["message"] = string.Format("{0} {1} has been saved!", user.FirstName, user.LastName);
                return RedirectToAction("List");
            }
            else
            {
                return View(user);
            }
            //try
            //{
            //    unitOfWork.Users.Update(user);
            //    unitOfWork.Save();

            //    return RedirectToAction("List");
            //}
            //catch
            //{
            //    ViewBag.Error = true;
            //    return View(user);
            //}
        }

        //
        // GET: /User/Delete/5

        //public ActionResult Delete(int id)
        //{
        //    var user = unitOfWork.Users.GetByID(id);
        //    return View(user);
        //}

        public User DeleteUser(int id)
        {
            User dbEntry = unitOfWork.Users.GetByID(id);
            if (dbEntry != null)
            {
                unitOfWork.Users.Delete(dbEntry);
                unitOfWork.Save();
            }

            return dbEntry;
        }

        //
        // POST: /User/Delete/5

        //[HttpPost] -??? Not working!
        public ActionResult Delete(int id)
        {
            User deletedUser = DeleteUser(id);
            if(deletedUser != null)
            {
                TempData["message"] = string.Format("{0} {1} was deleted!", deletedUser.FirstName, deletedUser.LastName);
            }

            return RedirectToAction("List");
        }

        public void SaveUser(User user)
        {
            if (user.Password != null)
            {
                user.Password = Crypto.HashPassword(user.Password);
            }

            if(user.ID == 0)
            {
                unitOfWork.Users.Insert(user);
            }
            else
            {
                User dbEntry = unitOfWork.Users.GetByID(user.ID);

                if(dbEntry != null)
                {
                    dbEntry.LastName = user.LastName;
                    dbEntry.FirstName = user.FirstName;
                    dbEntry.Login = user.Login;

                    if (user.Password != null)
                    {
                        dbEntry.Password = user.Password;
                    }
                    dbEntry.Role = user.Role;
                }
                //unitOfWork.Users.Update(user);
            }
            unitOfWork.Save();
        }
    }
}
