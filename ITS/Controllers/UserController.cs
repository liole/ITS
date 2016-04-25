using ITS.Domain.UnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork;
using ITS.Models;

namespace ITS.Controllers
{
    public class UserController : Controller
    {
        private IUnitOfWork unitOfWork;

        public int PageSize = 4;

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

        public ActionResult Create()
        {
            return View(new User());
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
            var user = unitOfWork.Users.GetByID(id);
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, User user)
        {
            try
            {
                unitOfWork.Users.Update(user);
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
        // GET: /User/Delete/5

        public ActionResult Delete(int id)
        {
            var user = unitOfWork.Users.GetByID(id);
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, User user)
        {
            try
            {
                unitOfWork.Users.Delete(id);
                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.Error = true;
                return View();
            }
        }
    }
}
