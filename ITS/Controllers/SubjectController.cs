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
    public class SubjectController : Controller
    {
        private IUnitOfWork unitOfWork;
        public int PageSize = 10;

        public SubjectController(IUnitOfWork uow)
        {
            this.unitOfWork = uow;
        }

        //
        // GET: /Subject/

        public ViewResult List(int page = 1)
        {
            SubjectsListViewModel model = new SubjectsListViewModel
            {
                Subjects = unitOfWork.Subjects.GetAll()
                .OrderBy(s => s.Name).ToList()
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = unitOfWork.Subjects.GetAll().Count()
                }
            };

            return View(model);
        }

        //
        // GET: /Subject/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Subject/Create

        public ViewResult Create()
        {
            return View("Edit", new Subject());
        }

        //
        // POST: /Subject/Create

        [HttpPost]
        public ActionResult Create(Subject subject)
        {
            try
            {
                unitOfWork.Subjects.Insert(subject);
                unitOfWork.Save();

                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.Error = true;
                return View(subject);
            }
        }

        //
        // GET: /Subject/Edit/5

        public ActionResult Edit(int id)
        {
            var subject = unitOfWork.Subjects.GetByID(id);

            return View(subject);
        }

        //
        // POST: /Subject/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Subject subject)
        {
           if(ModelState.IsValid)
           {
               SaveSubject(subject);
               TempData["message"] = string.Format("Subject {0} has been saved!", subject.Name);

               return RedirectToAction("List");
           }
           else
           {
               return View(subject);
           }
        }

        public Subject DeleteSubject(int id)
        {
            Subject dbEntry = unitOfWork.Subjects.GetByID(id);
            if(dbEntry != null)
            {
                unitOfWork.Subjects.Delete(dbEntry);
                unitOfWork.Save();
            }

            return dbEntry;
        }

        //
        // POST: /Subject/Delete/5

        //[HttpPost]
        public ActionResult Delete(int id)
        {
            Subject deletedSubject = DeleteSubject(id);
            if(deletedSubject != null)
            {
                TempData["message"] = string.Format("Subject {0} has been deleted!", deletedSubject.Name);
            }

            return RedirectToAction("List");
        }

        public void SaveSubject(Subject subject)
        {
            if(subject.ID == 0)
            {
                unitOfWork.Subjects.Insert(subject);
            }
            else
            {
                Subject dbEntry = unitOfWork.Subjects.GetByID(subject.ID);

                if(dbEntry != null)
                {
                    dbEntry.Name = subject.Name;
                }
            }

            unitOfWork.Save();
        }
    }
}
