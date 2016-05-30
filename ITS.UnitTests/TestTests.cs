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


namespace ITS.UnitTests
{
	[TestClass]
	public class TestTests
	{
		private Mock<IGenericRepository<Test>> mockRepository;
		private IUnitOfWork unitOfWrok;
		private List<Test> tests;
		private User currentUser;

		private TestController controller;

		[TestInitialize]
		public void InitializeUnitOfWork()
		{
			tests = new List<Test>() {
				new Test() {
					ID = 1,
					Mark = 12,
					Name = "Math",
					Randomize = true,
					UserID = 1
				},
				new Test() {
					ID = 2,
					Mark = 100,
					Name = "Programming",
					Randomize = false,
					UserID = 1
				},
				new Test() {
					ID = 3,
					Mark = 50,
					Name = "History",
					Randomize = true,
					UserID = 2
				}
			};
			currentUser = new User()
			{
				ID = 1,
				FirstName = "Test",
				LastName = "User",
				Login = "test1",
				IsTeacher = true,
				Tests = tests.Where(t => t.UserID == 1).ToList()
			};

			mockRepository = new Mock<IGenericRepository<Test>>();
			mockRepository.Setup(r => r.GetAll()).Returns(tests.AsQueryable());
			mockRepository.Setup(r => r.GetByID(It.IsAny<int>())).Returns<int>(id =>
				tests.FirstOrDefault(q => q.ID == id));

			var mockUserRepo = new Mock<IGenericRepository<User>>();
			mockUserRepo.Setup(r => r.GetByID(It.Is<int>(id => id == currentUser.ID)))
				.Returns<int>(id => currentUser);

			var mockUow = new Mock<IUnitOfWork>();
			mockUow.Setup(u => u.Tests).Returns(mockRepository.Object);
			mockUow.Setup(u => u.Users).Returns(mockUserRepo.Object);
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
	}
}
