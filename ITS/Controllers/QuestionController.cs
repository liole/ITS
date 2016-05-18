using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork.Abstract;
using ITS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Controllers
{
	[Auth(UserRole.Teacher)]
    public class QuestionController : Controller
    {
        private IUnitOfWork unitOfWork;

		public QuestionController(IUnitOfWork uow)
		{
			this.unitOfWork = uow;
		}

		public ActionResult Edit(int id)
		{
			var question = unitOfWork.Questions.GetByID(id);
			ViewBag.Create = false;
			if (question is ABCDQuestion)
			{
				ViewBag.ABCDList = selectABCDList((question as ABCDQuestion).Answer);
				return View("ABCDEditor", question);
			}
			if (question is TextQuestion)
			{
				return View("TextEditor", question);
			}
			if (question is NumberQuestion)
			{
				return View("NumberEditor", question);
			}
			throw new InvalidOperationException();
		}

		public ActionResult CreateABCD(int id)
		{
			var question = new ABCDQuestion()
			{
				TestID = id
			};
			ViewBag.Create = true;
			ViewBag.ABCDList = selectABCDList(ABCDAnswer.A);
			return View("ABCDEditor", question);
		}
		public ActionResult CreateText(int id)
		{
			var question = new TextQuestion()
			{
				TestID = id
			};
			ViewBag.Create = true;
			return View("TextEditor", question);
		}
		public ActionResult CreateNumber(int id)
		{
			var question = new NumberQuestion()
			{
				TestID = id
			};
			ViewBag.Create = true;
			return View("NumberEditor", question);
		}

		[HttpPost]
		public ActionResult EditABCD(ABCDQuestion question)
		{
			return Edit(question);
		}
		[HttpPost]
		public ActionResult EditText(TextQuestion question)
		{
			return Edit(question);
		}
		[HttpPost]
		public ActionResult EditNumber(NumberQuestion question)
		{
			return Edit(question);
		}

		[HttpPost]
		public ActionResult CreateABCD(ABCDQuestion question)
		{
			return Create(question);
		}
		[HttpPost]
		public ActionResult CreateText(TextQuestion question)
		{
			return Create(question);
		}
		[HttpPost]
		public ActionResult CreateNumber(NumberQuestion question)
		{
			return Create(question);
		}


		[HttpPost]
		public ActionResult Edit(Question question)
		{
			unitOfWork.Questions.Update(question);
			unitOfWork.Save();
			return RedirectToAction("Details", "Test", new { id = question.TestID });
		}

		[HttpPost]
		public ActionResult Create(Question question)
		{
			unitOfWork.Questions.Insert(question);
			unitOfWork.Save();
			return RedirectToAction("Details", "Test", new { id = question.TestID });
		}

		public ActionResult Delete(int id)
		{
			var question = unitOfWork.Questions.GetByID(id);
			var testID = question.TestID;
			unitOfWork.Questions.Delete(question);
			unitOfWork.Save();
			return RedirectToAction("Details", "Test", new { id = testID });
		}

		private List<SelectListItem> selectABCDList(ABCDAnswer selected)
		{
			var list = new List<SelectListItem>();
			for (var i = 0; i < 4; ++i)
			{
				list.Add(new SelectListItem() { 
					Text = ((char)('А' + i)).ToString(),
					Value = i.ToString(),
					Selected = (int)selected == i
				});
			}
			return list;
		}

    }
}
