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
    public class UserTests
    {
        private Mock<IGenericRepository<User>> mockRepository;
        private Mock<IGenericRepository<Group>> gMockRepository;
        private IUnitOfWork unitOfWrok;
        private List<User> users;
        private List<Group> groups;
        private User currentUser;

        private UserController controller;

        [TestInitialize]
        public void InitializeUnitOfWork()
        {
            users = new List<User>() {
				new User() {
					ID = 1,
					FirstName = "John",
					LastName = "Stuart"
				},
				new User() {
					ID = 2,
					FirstName = "Sam",
					LastName = "Jones"
				},
				new User() {
					ID = 3,
					FirstName = "Bob",
					LastName = "Samuel"
				}
			};

            groups = new List<Group>()
            {
                new Group()
                {
                    ID = 1,
                    Name = "Group1"
                },
                new Group()
                {
                    ID = 2,
                    Name = "Group2"
                }
            };
            currentUser = users[0];

            gMockRepository = new Mock<IGenericRepository<Group>>();
            gMockRepository.Setup(r => r.GetAll()).Returns(groups.AsQueryable());
            gMockRepository.Setup(r => r.GetByID(It.IsAny<int>())).Returns<int>(id =>
                groups.FirstOrDefault(q => q.ID == id));

            mockRepository = new Mock<IGenericRepository<User>>();
            mockRepository.Setup(r => r.GetAll()).Returns(users.AsQueryable());
            mockRepository.Setup(r => r.GetByID(It.IsAny<int>())).Returns<int>(id =>
                users.FirstOrDefault(q => q.ID == id));

            //var mockUserRepo = new Mock<IGenericRepository<User>>();
            //mockUserRepo.Setup(r => r.GetByID(It.Is<int>(id => id == currentUser.ID)))
            //    .Returns<int>(id => currentUser);

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.Users).Returns(mockRepository.Object);
            mockUow.Setup(u => u.Groups).Returns(gMockRepository.Object);
            mockUow.Setup(u => u.Save()).Callback(mockRepository.Object.Save);
            unitOfWrok = mockUow.Object;

            controller = new UserController(unitOfWrok);
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
        public void GeneralEdit()
        {
            var res = controller.Edit(2);
            Assert.IsInstanceOfType(res, typeof(ViewResult));
            var viewRes = res as ViewResult;

            Assert.AreEqual(users[1], viewRes.Model);
            //Assert.AreEqual("TextEditor", viewRes.ViewName);
            Assert.IsFalse(viewRes.ViewBag.Create);
        }

        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            Mock<IGenericRepository<User>> mockR = new Mock<IGenericRepository<User>>();
            mockR.Setup(r => r.GetAll()).Returns(() => new User[]
                {
                    new User{ID = 1, FirstName = "N1"},
                    new User{ID = 2, FirstName = "N2"},
                    new User{ID = 3, FirstName = "N3"},
                    new User{ID = 4, FirstName = "N4"},
                    new User{ID = 5, FirstName = "N5"}
                }.AsQueryable());
            mock.Setup(u => u.Users).Returns(mockR.Object);
            UserController controller = new UserController(mock.Object);
            controller.PageSize = 3;
            UsersListViewModel result = (UsersListViewModel)controller.List(2).Model;

            User[] userArray = result.Users.ToArray();
            Assert.IsTrue(userArray.Length == 2);
            Assert.AreEqual(userArray[0].FirstName, "N4");
            Assert.AreEqual(userArray[1].FirstName, "N5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            Mock<IGenericRepository<User>> mockR = new Mock<IGenericRepository<User>>();
            mockR.Setup(r => r.GetAll()).Returns(() => new User[]
                {
                    new User{ID = 1, FirstName = "N1"},
                    new User{ID = 2, FirstName = "N2"},
                    new User{ID = 3, FirstName = "N3"},
                    new User{ID = 4, FirstName = "N4"},
                    new User{ID = 5, FirstName = "N5"}
                }.AsQueryable());
            mock.Setup(u => u.Users).Returns(mockR.Object);
            UserController controller = new UserController(mock.Object);
            controller.PageSize = 3;

            UsersListViewModel result = (UsersListViewModel)controller.List(2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void CreateUser()
        {
            var res = controller.Create();
            Assert.IsInstanceOfType(res, typeof(ViewResult));
            var viewRes = res as ViewResult;

            Assert.IsInstanceOfType(viewRes.Model, typeof(User));
            Assert.IsTrue(viewRes.ViewBag.Create);
        }
        [TestMethod]
        public void PostCreateUser()
        {
            var res = controller.Create(users[0]);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            mockRepository.Verify(r => r.Insert(users[0]));
            mockRepository.Verify(r => r.Save());
        }
        [TestMethod]
        public void PostEditUser()
        {
            var res = controller.Edit(1,users[0] as User);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            //mockRepository.Verify(r => r.Update(users[0]));
            mockRepository.Verify(r => r.Save());
        }

        [TestMethod]
        public void DeleteUser()
        {
            var res = controller.Delete(1);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            mockRepository.Verify(r => r.Delete(users[0]));
            mockRepository.Verify(r => r.Save());
        }
    }
}
