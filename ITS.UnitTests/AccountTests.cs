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
using System.Web.Helpers;

namespace ITS.UnitTests
{
	[TestClass]
	public class AccountTests
	{
		private IUnitOfWork unitOfWrok;
		private Mock<IGenericRepository<User>> mockUserRepo; 
		private User currentUser;

		private AccountController controller;

		[TestInitialize]
		public void InitializeUnitOfWork()
		{
			currentUser = new User()
			{
				ID = 1,
				FirstName = "Test",
				LastName = "User",
				Login = "test1",
				Password = Crypto.HashPassword("pas")
			};

			mockUserRepo = new Mock<IGenericRepository<User>>();
			mockUserRepo.Setup(r => r.GetByID(It.Is<int>(id => id == currentUser.ID)))
				.Returns<int>(id => currentUser);
			mockUserRepo.Setup(r => r.GetAll()).Returns(new List<User>() { currentUser }.AsQueryable());


			var mockUow = new Mock<IUnitOfWork>();
			mockUow.Setup(u => u.Users).Returns(mockUserRepo.Object);
			mockUow.Setup(u => u.Save()).Callback(mockUserRepo.Object.Save);
			unitOfWrok = mockUow.Object;

			controller = new AccountController(unitOfWrok);
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
		public void GetProfile()
		{
			var res = controller.Profile();
			Assert.IsInstanceOfType(res, typeof(ViewResult));
			var viewRes = res as ViewResult;

			Assert.IsInstanceOfType(viewRes.Model, typeof(UserProfilesViewModel));
			var profile = viewRes.Model as UserProfilesViewModel;
			Assert.AreEqual(currentUser.FirstName, profile.FirstName);
			Assert.AreEqual(currentUser.LastName, profile.LastName);
			Assert.AreEqual(currentUser.Login, profile.Login);
		}

		[TestMethod]
		public void PostProfile()
		{
			var res = controller.Profile(new UserProfilesViewModel() { 
				LastName = "ln",
				FirstName = "fn"
			});
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
			var redirectRes = res as RedirectToRouteResult;

			Assert.AreEqual("Profile", redirectRes.RouteValues["action"]);

			Assert.AreEqual("fn", currentUser.FirstName);
			Assert.AreEqual("ln", currentUser.LastName);

			mockUserRepo.Verify(r => r.Update(currentUser));
			mockUserRepo.Verify(r => r.Save());
		}

		[TestMethod]
		public void Login()
		{
			var res = controller.Login();
			Assert.IsInstanceOfType(res, typeof(ViewResult));
		}

		[TestMethod]
		public void PostLoginOk()
		{
			var backup = currentUser;
			currentUser = null;
			updateCurrentUser();
			currentUser = backup;
			var res = controller.Login("test1", "pas");
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));

			CollectionAssert.Contains(controller.Session.Keys, "user");
		}

		[TestMethod]
		public void PostLoginFail()
		{
			currentUser = null;
			updateCurrentUser();
			var res = controller.Login("test1", "none");
			Assert.IsInstanceOfType(res, typeof(ViewResult));

			CollectionAssert.DoesNotContain(controller.Session.Keys, "user");
		}

		[TestMethod]
		public void Logout()
		{
			var res = controller.Logout();
			Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));

			CollectionAssert.DoesNotContain(controller.Session.Keys, "user");
		}
	}
}
