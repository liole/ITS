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
    public class GroupController : Controller
    {
        //
        // GET: /Group/
        private IUnitOfWork unitOfWork;

        public int PageSize = 10;

        public GroupController(IUnitOfWork uof)
        {
            this.unitOfWork = uof;
        }

        public ViewResult List(int page = 1)
        {
            GroupsListViewModel model = new GroupsListViewModel
            {
                Groups = unitOfWork.Groups.GetAll()
                .OrderBy(o => o.Name).ToList()
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
        // GET: /Group/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Group/Create

        public ActionResult Create()
        {
            return View(new Group());
        }

        //
        // POST: /Group/Create

        [HttpPost]
        public ActionResult Create(Group group)
        {
            try
            {
                unitOfWork.Groups.Insert(group);
                unitOfWork.Save();

                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.Error = true;
                return View(group);
            }
        }

        //
        // GET: /Group/Edit/5

        public ActionResult Edit(int id)
        {
            var group = unitOfWork.Groups.GetByID(id);

            return View(group);
        }

        //
        // POST: /Group/Edit/5

        //[HttpPost]
        //public ActionResult Edit(int id, Group group)
        //{
        //    try
        //    {
        //        unitOfWork.Groups.Update(group);
        //        unitOfWork.Save();

        //        return RedirectToAction("List");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //
        // GET: /Group/Delete/5

        public ActionResult Delete(int id)
        {
            var group = unitOfWork.Groups.GetByID(id);
            return View(group);
        }

        //
        // POST: /Group/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                unitOfWork.Groups.Delete(id);

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
