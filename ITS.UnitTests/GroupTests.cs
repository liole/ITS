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
    public class GroupTests
    {
        private Mock<IGenericRepository<User>> mockRepository;
        private Mock<IGenericRepository<Group>> gMockRepository;
        private IUnitOfWork unitOfWrok;
        private List<User> users;
        private List<Group> groups;
        private User currentUser;

        private GroupController controller;

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
                },
                new Group()
                {
                    ID = 3,
                    Name = "Group3"
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
            mockUow.Setup(u => u.Save()).Callback(gMockRepository.Object.Save);
            unitOfWrok = mockUow.Object;

            controller = new GroupController(unitOfWrok);
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

            Assert.AreEqual(groups[1], viewRes.Model);
            //Assert.AreEqual("TextEditor", viewRes.ViewName);
            //Assert.IsFalse(viewRes.ViewBag.Create);
        }

        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            Mock<IGenericRepository<Group>> mockR = new Mock<IGenericRepository<Group>>();
            mockR.Setup(r => r.GetAll()).Returns(() => new Group[]
                {
                    new Group{ID = 1, Name = "N1"},
                    new Group{ID = 2, Name = "N2"},
                    new Group{ID = 3, Name = "N3"},
                    new Group{ID = 4, Name = "N4"},
                    new Group{ID = 5, Name = "N5"}
                }.AsQueryable());
            mock.Setup(u => u.Groups).Returns(mockR.Object);
            GroupController controller = new GroupController(mock.Object);
            controller.PageSize = 3;
            GroupsListViewModel result = (GroupsListViewModel)controller.List(2).Model;

            Group[] groupArray = result.Groups.ToArray();
            Assert.IsTrue(groupArray.Length == 2);
            Assert.AreEqual(groupArray[0].Name, "N4");
            Assert.AreEqual(groupArray[1].Name, "N5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            Mock<IGenericRepository<Group>> mockR = new Mock<IGenericRepository<Group>>();
            mockR.Setup(r => r.GetAll()).Returns(() => new Group[]
                {
                    new Group{ID = 1, Name = "N1"},
                    new Group{ID = 2, Name = "N2"},
                    new Group{ID = 3, Name = "N3"},
                    new Group{ID = 4, Name = "N4"},
                    new Group{ID = 5, Name = "N5"}
                }.AsQueryable());
            mock.Setup(u => u.Groups).Returns(mockR.Object);
            GroupController controller = new GroupController(mock.Object);
            controller.PageSize = 3;

            GroupsListViewModel result = (GroupsListViewModel)controller.List(2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void CreateGroup()
        {
            var res = controller.Create();
            Assert.IsInstanceOfType(res, typeof(ViewResult));
            var viewRes = res as ViewResult;

            Assert.IsInstanceOfType(viewRes.Model, typeof(Group));
            //Assert.IsFalse(viewRes.ViewBag.Create);
        }

        [TestMethod]
        public void PostCreateGroup()
        {
            var res = controller.Create(groups[0]);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            gMockRepository.Verify(r => r.Insert(groups[0]));
            gMockRepository.Verify(r => r.Save());
        }

        [TestMethod]
        public void PostEditUser()
        {
            var res = controller.Edit(1, groups[0] as Group);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            //gMockRepository.Verify(r => r.Update(groups[0]));
            gMockRepository.Verify(r => r.Save());
        }

        [TestMethod]
        public void DeleteGroup()
        {
            var res = controller.Delete(1);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            gMockRepository.Verify(r => r.Delete(groups[0]));
            gMockRepository.Verify(r => r.Save());
        }
        
    }
}
