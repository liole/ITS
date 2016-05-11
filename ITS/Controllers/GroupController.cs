using ITS.Domain.UnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork;
using ITS.Models;
using ITS.Infrastructure;

namespace ITS.Controllers
{
	[Auth(UserRole.Admin)]
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

        public ViewResult Create()
        {
            return View("Edit", new Group());
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

        [HttpPost]
        public ActionResult Edit(int id, Group group)
        {
            if (ModelState.IsValid)
            {
                SaveGroup(group);
                TempData["message"] = string.Format("Group {0} has been saved!", group.Name);
                return RedirectToAction("List");
            }
            else
            {
                return View(group);
            }
        }

        //
        // GET: /Group/Delete/5

        //public ActionResult Delete(int id)
        //{
        //    var group = unitOfWork.Groups.GetByID(id);
        //    return View(group);
        //}

        public Group DeleteGroup(int id)
        {
            Group dbEntry = unitOfWork.Groups.GetByID(id);
            if (dbEntry != null)
            {
                unitOfWork.Groups.Delete(dbEntry);
                unitOfWork.Save();
            }

            return dbEntry;
        }

        //
        // POST: /Group/Delete/5

        //[HttpPost]
        public ActionResult Delete(int id)
        {
            Group deletedGroup = DeleteGroup(id);
            if (deletedGroup != null)
            {
                TempData["message"] = string.Format("Group {0} was deleted!", deletedGroup.Name);
            }

            return RedirectToAction("List");
        }

        public void SaveGroup(Group group)
        {

            if (group.ID == 0)
            {
                unitOfWork.Groups.Insert(group);
            }
            else
            {
                Group dbEntry = unitOfWork.Groups.GetByID(group.ID);

                if (dbEntry != null)
                {
                    dbEntry.Name = group.Name;
                }
            }
            unitOfWork.Save();
        }

        public ActionResult Users(int id)
        {
            var group1 = unitOfWork.Groups.GetByID(id);

            return View(group1);
        }

        public ActionResult RemoveUser(int groupId, int userId)
        {
            var user1 = unitOfWork.Users.GetByID(userId);
            var group1 = unitOfWork.Groups.GetByID(groupId);

            group1.Users.Remove(user1);
            unitOfWork.Save();

            if(group1.Users.Count() == 0)
            {
                return RedirectToAction("List");
            }

            return RedirectToAction("Users", new { id = groupId });

        }
    }
}
