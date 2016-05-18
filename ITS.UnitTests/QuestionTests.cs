using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITS.Domain.UnitOfWork.Abstract;
using ITS.Domain.Entities;
using Moq;
using System.Collections.Generic;
using ITS.Controllers;
using System.Web.Mvc;
using System.Web;

namespace ITS.UnitTests
{
	[TestClass]
	public class QuestionTests
	{
		private Mock<IGenericRepository<Question>> mockRepository;
		private IUnitOfWork unitOfWrok;
		private List<Question> questions;

		private QuestionController controller;

		[TestInitialize]
		public void InitializeUnitOfWork()
		{
			questions = new List<Question>() {
				new ABCDQuestion()
				{
					ID = 1,
					QuestionText = "Test 1",
					Coefficient = 1,
					TestID = 2,
					AnswerA = "A",
					AnswerB = "B",
					AnswerC = "C",
					AnswerD = "D",
					Answer = ABCDAnswer.B
				},
				new TextQuestion()
				{
					ID = 2,
					QuestionText = "Test 2",
					Coefficient = 2,
					TestID = 2,
					Answer = "answer"
				},
				new NumberQuestion()
				{
					ID = 3,
					QuestionText = "Test 3",
					Coefficient = 1,
					TestID = 1,
					Answer = 2.5M
				}
			};

			mockRepository = new Mock<IGenericRepository<Question>>();
			mockRepository.Setup(r => r.GetAll()).Returns(questions.AsQueryable());
			mockRepository.Setup(r => r.GetByID(It.IsAny<int>())).Returns<int>(id => 
				questions.FirstOrDefault(q => q.ID == id));

			var mockUow = new Mock<IUnitOfWork>();
			mockUow.Setup(u => u.Questions).Returns(mockRepository.Object);
			mockUow.Setup(u => u.Save()).Callback(mockRepository.Object.Save);
			unitOfWrok = mockUow.Object;

			controller = new QuestionController(unitOfWrok);
		}

		[TestMethod]
		public void GeneralEdit()
		{
			var res = controller.Edit(2);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.AreEqual(questions[1], viewRes.Model);
			Assert.AreEqual("TextEditor", viewRes.ViewName);
			Assert.IsFalse(viewRes.ViewBag.Create);
		}

		[TestMethod]
		public void GeneralEditABCDList()
		{
			var res = controller.Edit(1);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.AreEqual(questions[0], viewRes.Model);
			Assert.AreEqual("ABCDEditor", viewRes.ViewName);
			Assert.IsFalse(viewRes.ViewBag.Create);
			Assert.AreEqual(4, viewRes.ViewBag.ABCDList.Count);

			var abcdList = viewRes.ViewBag.ABCDList as List<SelectListItem>;
			foreach(var item in abcdList)
			{
				Assert.AreEqual(
					(ABCDAnswer)int.Parse(item.Value) == (questions[0] as ABCDQuestion).Answer,
					item.Selected
				);
			}
		}

		[TestMethod]
		public void CreateViewABCD()
		{
			var res = controller.CreateABCD(5);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(ABCDQuestion));
			Assert.AreEqual(5, (viewRes.Model as Question).TestID);
			Assert.IsTrue(viewRes.ViewBag.Create);
		}

		[TestMethod]
		public void CreateViewText()
		{
			var res = controller.CreateText(7);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(TextQuestion));
			Assert.AreEqual(7, (viewRes.Model as Question).TestID);
			Assert.IsTrue(viewRes.ViewBag.Create);
		}

		[TestMethod]
		public void CreateViewNumber()
		{
			var res = controller.CreateNumber(3);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(NumberQuestion));
			Assert.AreEqual(3, (viewRes.Model as Question).TestID);
			Assert.IsTrue(viewRes.ViewBag.Create);
		}

		[TestMethod]
		public void PostEditQuestion()
		{
			var res = controller.Edit(questions[1]);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[1].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Update(questions[1]));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void PostEditABCD()
		{
			var res = controller.EditABCD(questions[0] as ABCDQuestion);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[0].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Update(It.IsAny<ABCDQuestion>()));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void PostEditText()
		{
			var res = controller.EditText(questions[1] as TextQuestion);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[1].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Update(It.IsAny<TextQuestion>()));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void PostEditNumber()
		{
			var res = controller.EditNumber(questions[2] as NumberQuestion);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[2].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Update(It.IsAny<NumberQuestion>()));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void PostCreateQuestion()
		{
			var res = controller.Create(questions[1]);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[1].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Insert(questions[1]));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void PostCreateABCD()
		{
			var res = controller.CreateABCD(questions[0] as ABCDQuestion);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[0].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Insert(It.IsAny<ABCDQuestion>()));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void PostCreateText()
		{
			var res = controller.CreateText(questions[1] as TextQuestion);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[1].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Insert(It.IsAny<TextQuestion>()));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void PostCreateNumber()
		{
			var res = controller.CreateNumber(questions[2] as NumberQuestion);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[2].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Insert(It.IsAny<NumberQuestion>()));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void DeleteQuestion()
		{
			var res = controller.Delete(2);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Test", redirectRes.RouteValues["controller"]);
			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(questions[1].TestID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Delete(questions[1]));
			mockRepository.Verify(r => r.Save());
		}
	}
}
