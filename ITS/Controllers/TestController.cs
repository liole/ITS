using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Controllers
{
    public class TestController : Controller
    {
		private IUnitOfWork unitOfWork;

		public TestController(IUnitOfWork uow)
		{
			this.unitOfWork = uow;
		}
 
        public ActionResult List()
        {
			var tests = CurrentUser().Tests;
            return View(tests);
        }

		public ActionResult Details(int id)
		{
			var test = unitOfWork.Tests.GetByID(id);
			return View(test);
		}

		public ActionResult Edit(int id)
		{
			var test = unitOfWork.Tests.GetByID(id);
			var subjects = unitOfWork.Subjects.GetAll().Select(s => new SelectListItem()
			{
				Text = s.Name,
				Value = s.ID.ToString(),
				Selected = s.ID == test.SubjectID
			});
			ViewBag.Create = false;
			ViewBag.Subjects = subjects;
			return View(test);
		}
		[HttpPost]
		public ActionResult Edit(Test test)
		{
			unitOfWork.Tests.Update(test);
			unitOfWork.Save();
			return RedirectToAction("Details", new { id = test.ID });
		}

		public ActionResult Create()
		{
			var test = new Test()
			{
				UserID = CurrentUser().ID
			};
			var subjects = unitOfWork.Subjects.GetAll().Select(s => new SelectListItem()
			{
				Text = s.Name,
				Value = s.ID.ToString()
			}); 
			ViewBag.Create = true;
			ViewBag.Subjects = subjects;
			return View("Edit", test);
		}
		[HttpPost]
		public ActionResult Create(Test test)
		{
			unitOfWork.Tests.Insert(test);
			unitOfWork.Save();
			return RedirectToAction("Details", new { id = test.ID });
		}

		public ActionResult Delete(int id)
		{
			unitOfWork.Tests.Delete(id);
			unitOfWork.Save();
			return RedirectToAction("List");
		}

		private User CurrentUser()
		{
			return unitOfWork.Users.GetByID(1);
		}

    }
}
