using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.SessionState;
using System.Web.Mvc;
using Moq;
using MvcFakes;
using ITS.Domain.UnitOfWork.Abstract;
using ITS.Domain.Entities;
using ITS.Controllers;
using ITS.Models;


namespace ITS.UnitTests
{
	[TestClass]
	public class TestTests
	{
		private Mock<IGenericRepository<Test>> mockRepository;
		private IUnitOfWork unitOfWrok;
		private List<Test> tests;
		private List<Subject> subjects;
		private List<Group> groups;
		private User currentUser;

		private TestController controller;

		[TestInitialize]
		public void InitializeUnitOfWork()
		{
			tests = new List<Test>()
			{
				new Test() {
					ID = 1,
					Mark = 12,
					Name = "Math",
					Randomize = true,
					UserID = 1,
					SubjectID = 1,
					Groups = new List<Group>(),
					Results = new List<Result>()
				},
				new Test() {
					ID = 2,
					Mark = 100,
					Name = "Programming",
					Randomize = false,
					UserID = 1,
					SubjectID = 2,
					Groups = new List<Group>(),
					Results = new List<Result>()
				},
				new Test() {
					ID = 3,
					Mark = 50,
					Name = "History",
					Randomize = true,
					UserID = 2,
					SubjectID = 1,
					Groups = new List<Group>(),
					Results = new List<Result>()
				}
			};
			subjects = new List<Subject>()
			{
				new Subject() {
					ID = 1,
					Name = "Math"
				},
				new Subject() {
					ID = 2,
					Name = "Programming"
				}
			};
			groups = new List<Group>()
			{
				new Group() {
					ID = 1,
					Name = "AMI-33",
					Tests = new List<Test>(){ tests[0], tests[1] }
				},
				new Group() {
					ID = 2,
					Name = "10-A",
					Tests = new List<Test>(){ tests[2] }
				}
			};
			currentUser = new User()
			{
				ID = 1,
				FirstName = "Test",
				LastName = "User",
				Login = "test1",
				IsTeacher = true,
				Tests = tests.Where(t => t.UserID == 1).ToList(),
				Groups = new List<Group>() { groups[0] }
			};
			tests[2].Groups.Add(groups[0]);

			mockRepository = new Mock<IGenericRepository<Test>>();
			mockRepository.Setup(r => r.GetAll()).Returns(tests.AsQueryable());
			mockRepository.Setup(r => r.GetByID(It.IsAny<int>())).Returns<int>(id =>
				tests.FirstOrDefault(q => q.ID == id));

			var mockUserRepo = new Mock<IGenericRepository<User>>();
			mockUserRepo.Setup(r => r.GetByID(It.Is<int>(id => id == currentUser.ID)))
				.Returns<int>(id => currentUser);

			var mockSubjectRepo = new Mock<IGenericRepository<Subject>>();
			mockSubjectRepo.Setup(r => r.GetAll()).Returns(subjects.AsQueryable());

			var mockGroupRepo = new Mock<IGenericRepository<Group>>();
			mockGroupRepo.Setup(r => r.GetAll()).Returns(groups.AsQueryable());
			mockGroupRepo.Setup(r => r.GetByID(It.IsAny<int>())).Returns<int>(id =>
				groups.FirstOrDefault(q => q.ID == id));

			var mockUow = new Mock<IUnitOfWork>();
			mockUow.Setup(u => u.Tests).Returns(mockRepository.Object);
			mockUow.Setup(u => u.Users).Returns(mockUserRepo.Object);
			mockUow.Setup(u => u.Subjects).Returns(mockSubjectRepo.Object);
			mockUow.Setup(u => u.Groups).Returns(mockGroupRepo.Object);
			mockUow.Setup(u => u.Save()).Callback(mockRepository.Object.Save);
			unitOfWrok = mockUow.Object;

			controller = new TestController(unitOfWrok);
			updateCurrentUser();
			
		}

		private void updateCurrentUser()
		{
			var sessionItems = new SessionStateItemCollection();
			if (currentUser != null)
			{
				sessionItems["user"] = currentUser.ID;
			}
			controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
		}

		[TestMethod]
		public void ListTest()
		{
			var res = controller.List();
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;
			var testsCount = tests.Count(t => t.UserID == currentUser.ID);

			Assert.IsInstanceOfType(viewRes.Model, typeof(ICollection<Test>));
			var collection = viewRes.Model as ICollection;
			Assert.AreEqual(testsCount, collection.Count);
			CollectionAssert.Contains(collection, tests[0]);
			CollectionAssert.Contains(collection, tests[1]);
			CollectionAssert.DoesNotContain(collection, tests[2]);
		}

		[TestMethod]
		public void Details()
		{
			var res = controller.Details(2);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(Test));
			Assert.AreEqual(tests[1], viewRes.Model);
		}

		[TestMethod]
		public void TakeView()
		{
			var res = controller.Take(3, true);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(Test));
			Assert.AreEqual(tests[2], viewRes.Model);
			Assert.AreEqual(true, viewRes.ViewBag.Save);
		}

		[TestMethod]
		public void EditView()
		{
			var res = controller.Edit(2);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(Test));
			Assert.AreEqual(tests[1], viewRes.Model);
			Assert.IsFalse(viewRes.ViewBag.Create);
			Assert.IsNotNull(viewRes.ViewBag.Subjects);
			Assert.IsInstanceOfType(viewRes.ViewBag.Subjects, typeof(IQueryable<SelectListItem>));
			var subjects = (IQueryable<SelectListItem>)(viewRes.ViewBag.Subjects);
			Assert.AreEqual(this.subjects.Count, subjects.Count());
			Assert.AreEqual("Programming", subjects.Single(s => s.Selected).Text);
		}

		[TestMethod]
		public void EditPost()
		{
			var res = controller.Edit(tests[0]);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(tests[0].ID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Update(tests[0]));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void CreateView()
		{
			var res = controller.Create();
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.AreEqual("Edit", viewRes.ViewName);

			Assert.IsInstanceOfType(viewRes.Model, typeof(Test));
			Assert.AreEqual(currentUser.ID, ((Test)viewRes.Model).UserID);
			Assert.IsTrue(viewRes.ViewBag.Create);
			Assert.IsNotNull(viewRes.ViewBag.Subjects);
			Assert.IsInstanceOfType(viewRes.ViewBag.Subjects, typeof(IQueryable<SelectListItem>));
			var subjects = (IQueryable<SelectListItem>)(viewRes.ViewBag.Subjects);
			Assert.AreEqual(this.subjects.Count, subjects.Count());
		}

		[TestMethod]
		public void CreatePost()
		{
			var res = controller.Create(tests[1]);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Details", redirectRes.RouteValues["action"]);
			Assert.AreEqual(tests[1].ID, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Insert(tests[1]));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void Delete()
		{
			var res = controller.Delete(1);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("List", redirectRes.RouteValues["action"]);

			mockRepository.Verify(r => r.Delete(1));
			mockRepository.Verify(r => r.Save());
		}

		[TestMethod]
		public void AssignView()
		{
			var res = controller.Assign(3);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(IQueryable<GroupTestViewModel>));
			var model = (IQueryable<GroupTestViewModel>)(viewRes.Model);
			Assert.AreEqual(groups.Count, model.Count());
			Assert.AreEqual(groups[1], model.Single(g => g.IsAssigned).Group);
		}

		[TestMethod]
		public void AddGroup()
		{
			var res = controller.AddGroup(3, 2);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Assign", redirectRes.RouteValues["action"]);
			Assert.AreEqual(3, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Update(tests[2]));
			mockRepository.Verify(r => r.Save());
			var collection = tests[2].Groups as ICollection;
			CollectionAssert.Contains(collection, groups[1]);
		}

		[TestMethod]
		public void RemoveGroup()
		{
			var res = controller.RemoveGroup(3, 1);
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Assign", redirectRes.RouteValues["action"]);
			Assert.AreEqual(3, redirectRes.RouteValues["id"]);

			mockRepository.Verify(r => r.Update(tests[2]));
			mockRepository.Verify(r => r.Save());
			var collection = tests[2].Groups as ICollection;
			CollectionAssert.DoesNotContain(collection, groups[0]);
		}

		[TestMethod]
		public void Results()
		{
			var res = controller.Results(1);
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(Test));
			Assert.AreEqual(tests[0], viewRes.Model);
		}

		[TestMethod]
		public void Assigned()
		{
			var res = controller.Assigned();
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.AreEqual("Index", viewRes.ViewName);

			Assert.IsInstanceOfType(viewRes.Model, typeof(MyTestsViewModel));
			var model = viewRes.Model as MyTestsViewModel;
			Assert.AreEqual(0, model.Past.Count());
			Assert.AreEqual(2, model.Present.Count());
		}
	}
}
