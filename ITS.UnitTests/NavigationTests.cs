using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.SessionState;
using System.Web.Mvc;
using System.Linq;
using Moq;
using MvcFakes;
using ITS.Domain.UnitOfWork.Abstract;
using ITS.Domain.Entities;
using ITS.Controllers;
using System.Collections.Generic;
using ITS.Models;

namespace ITS.UnitTests
{
	[TestClass]
	public class NavigationTests
	{
		private IUnitOfWork unitOfWrok;
		private User currentUser;

		private NavigationController controller;

		[TestInitialize]
		public void InitializeUnitOfWork()
		{
			currentUser = new User()
			{
				ID = 1,
				FirstName = "Test",
				LastName = "User",
				Login = "test1"
			};

			var mockUserRepo = new Mock<IGenericRepository<User>>();
			mockUserRepo.Setup(r => r.GetByID(It.Is<int>(id => id == currentUser.ID)))
				.Returns<int>(id => currentUser);


			var mockUow = new Mock<IUnitOfWork>();
			mockUow.Setup(u => u.Users).Returns(mockUserRepo.Object);
			unitOfWrok = mockUow.Object;

			controller = new NavigationController(unitOfWrok);
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
		public void NotAuthorisedMenu()
		{
			currentUser = null;
			updateCurrentUser();

			var res = controller.Menu();
			Assert.IsInstanceOfType(res, typeof(PartialViewResult));
			var viewRes = res as PartialViewResult;

			Assert.IsNull(viewRes.Model);
		}

		[TestMethod]
		public void AdminMenu()
		{
			currentUser.IsAdmin = true;
			updateCurrentUser();

			var res = controller.Menu();
			Assert.IsInstanceOfType(res, typeof(PartialViewResult));
			var viewRes = res as PartialViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(IEnumerable<LinkViewModel>));
			var menu = viewRes.Model as IEnumerable<LinkViewModel>;

			Assert.AreEqual(6, menu.Count());
		}

		[TestMethod]
		public void TeacherMenu()
		{
			currentUser.IsTeacher = true;
			updateCurrentUser();

			var res = controller.Menu();
			Assert.IsInstanceOfType(res, typeof(PartialViewResult));
			var viewRes = res as PartialViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(IEnumerable<LinkViewModel>));
			var menu = viewRes.Model as IEnumerable<LinkViewModel>;

			Assert.AreEqual(4, menu.Count());
		}

		[TestMethod]
		public void StudentMenu()
		{
			currentUser.IsStudent = true;
			updateCurrentUser();

			var res = controller.Menu();
			Assert.IsInstanceOfType(res, typeof(PartialViewResult));
			var viewRes = res as PartialViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(IEnumerable<LinkViewModel>));
			var menu = viewRes.Model as IEnumerable<LinkViewModel>;

			Assert.AreEqual(4, menu.Count());
		}

		[TestMethod]
		public void AdminTeacherMenu()
		{
			currentUser.IsAdmin = true;
			currentUser.IsTeacher = true;
			updateCurrentUser();

			var res = controller.Menu();
			Assert.IsInstanceOfType(res, typeof(PartialViewResult));
			var viewRes = res as PartialViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(IEnumerable<LinkViewModel>));
			var menu = viewRes.Model as IEnumerable<LinkViewModel>;

			Assert.AreEqual(7, menu.Count());
		}

		[TestMethod]
		public void AdminStudentMenu()
		{
			currentUser.IsAdmin = true;
			currentUser.IsStudent = true;
			updateCurrentUser();

			var res = controller.Menu();
			Assert.IsInstanceOfType(res, typeof(PartialViewResult));
			var viewRes = res as PartialViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(IEnumerable<LinkViewModel>));
			var menu = viewRes.Model as IEnumerable<LinkViewModel>;

			Assert.AreEqual(8, menu.Count());
		}

		[TestMethod]
		public void TeacherStudentMenu()
		{
			currentUser.IsTeacher = true;
			currentUser.IsStudent = true;
			updateCurrentUser();

			var res = controller.Menu();
			Assert.IsInstanceOfType(res, typeof(PartialViewResult));
			var viewRes = res as PartialViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(IEnumerable<LinkViewModel>));
			var menu = viewRes.Model as IEnumerable<LinkViewModel>;

			Assert.AreEqual(6, menu.Count());
		}

		[TestMethod]
		public void FullMenu()
		{
			currentUser.IsTeacher = true;
			currentUser.IsStudent = true;
			currentUser.IsAdmin = true;
			updateCurrentUser();

			var res = controller.Menu();
			Assert.IsInstanceOfType(res, typeof(PartialViewResult));
			var viewRes = res as PartialViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(IEnumerable<LinkViewModel>));
			var menu = viewRes.Model as IEnumerable<LinkViewModel>;

			Assert.AreEqual(9, menu.Count());
		}
	}
}
