using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Models;
using ITS.Infrastructure;

namespace ITS.Controllers
{
	[Auth(UserRole.Teacher)]
    public class TestController : Controller
    {
		private IUnitOfWork unitOfWork;

		public TestController(IUnitOfWork uow)
		{
			this.unitOfWork = uow;
		}

		[Auth(UserRole.Student)]
		public ActionResult Assigned()
		{
			var user = CurrentUser();
			var tests = user.Groups.SelectMany(g => g.Tests);
			var past = tests.Where(t => t.Results.Any(r => r.UserID == user.ID));
			var present = tests.Where(t => t.Results.All(r => r.UserID != user.ID));
			var model = new MyTestsViewModel()
			{
				Past = past.Select(t => new MarkedTest()
				{
					Test = t,
					Mark = t.Results.Last(r => r.UserID == user.ID).Mark
				}),
				Present = present
			};
			return View("Index", model);
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

		[Auth(UserRole.Student, UserRole.Teacher)]
		public ActionResult Take(int id, bool save = true)
		{
			var test = unitOfWork.Tests.GetByID(id);
			ViewBag.Save = save;
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

		public ActionResult Assign(int id)
		{
			var groups = unitOfWork.Groups.GetAll();
			var model = groups.Select(g => new GroupTestViewModel() { 
				Group = g,
				IsAssigned = g.Tests.Any(t => t.ID == id)
			});
			ViewBag.TestId = id;
			return View(model);
		}

		public ActionResult AddGroup(int id, int groupId)
		{
			var test = unitOfWork.Tests.GetByID(id);
			var group = unitOfWork.Groups.GetByID(groupId);
			test.Groups.Add(group);
			unitOfWork.Tests.Update(test);
			unitOfWork.Save();
			return RedirectToAction("Assign", new { id = id });
		}

		public ActionResult RemoveGroup(int id, int groupId)
		{
			var test = unitOfWork.Tests.GetByID(id);
			var group = unitOfWork.Groups.GetByID(groupId);
			test.Groups.Remove(group);
			unitOfWork.Tests.Update(test);
			unitOfWork.Save();
			return RedirectToAction("Assign", new { id = id });
		}

		[HttpPost]
		[Auth(UserRole.Student, UserRole.Teacher)]
		public ActionResult Check(TestAnswersModel answers, bool save = true)
		{
			//return Newtonsoft.Json.JsonConvert.SerializeObject(answers);
			var test = unitOfWork.Tests.GetByID(answers.TestID);
			decimal sum = 0;
			decimal max = 0;
			int count = 0;
			foreach(var ans in answers.Questions)
			{
				var question = unitOfWork.Questions.GetByID(ans.ID);
				var right = false;
				if (question is ABCDQuestion)
				{
					if ((question as ABCDQuestion).Answer.ToString() == ans.Answer)
						right = true;
				}
				if (question is TextQuestion)
				{
					if ((question as TextQuestion).Answer == ans.Answer)
						right = true;
				}
				max += question.Coefficient;
				if (right)
				{
					count++;
					sum += question.Coefficient;
				}
			}
			int result = (int)Math.Round(sum / max * test.Mark);
			if (save)
			{
				var user = CurrentUser();
				test.Results.Add(new Result(){
					Mark = result,
					QuestionAmountCorrect = count,
					UserID = user.ID
				});
				unitOfWork.Tests.Update(test);
				unitOfWork.Save();
			}
			return View("Result", new TestResultViewModel()
			{
				Test = test,
				Count = count,
				Result = result
			});
		}

		public ActionResult Results(int id)
		{
			var test = unitOfWork.Tests.GetByID(id);

			return View(test);
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
