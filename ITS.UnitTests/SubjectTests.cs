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
    public class SubjectTests
    {
        private Mock<IGenericRepository<User>> mockRepository;
        private Mock<IGenericRepository<Subject>> sMockRepository;
        private IUnitOfWork unitOfWrok;
        private List<User> users;
        private List<Subject> subjects;
        private User currentUser;

        private SubjectController controller;

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

            subjects = new List<Subject>()
            {
                new Subject()
                {
                    ID = 1,
                    Name = "Subject1"
                },
                new Subject()
                {
                    ID = 2,
                    Name = "Subject2"
                }
            };
            currentUser = users[0];

            sMockRepository = new Mock<IGenericRepository<Subject>>();
            sMockRepository.Setup(r => r.GetAll()).Returns(subjects.AsQueryable());
            sMockRepository.Setup(r => r.GetByID(It.IsAny<int>())).Returns<int>(id =>
                subjects.FirstOrDefault(q => q.ID == id));

            mockRepository = new Mock<IGenericRepository<User>>();
            mockRepository.Setup(r => r.GetAll()).Returns(users.AsQueryable());
            mockRepository.Setup(r => r.GetByID(It.IsAny<int>())).Returns<int>(id =>
                users.FirstOrDefault(q => q.ID == id));

            //var mockUserRepo = new Mock<IGenericRepository<User>>();
            //mockUserRepo.Setup(r => r.GetByID(It.Is<int>(id => id == currentUser.ID)))
            //    .Returns<int>(id => currentUser);

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.Users).Returns(mockRepository.Object);
            mockUow.Setup(u => u.Subjects).Returns(sMockRepository.Object);
            mockUow.Setup(u => u.Save()).Callback(mockRepository.Object.Save);
            mockUow.Setup(u => u.Save()).Callback(sMockRepository.Object.Save);
            unitOfWrok = mockUow.Object;

            controller = new SubjectController(unitOfWrok);
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

            Assert.AreEqual(subjects[1], viewRes.Model);
            //Assert.AreEqual("TextEditor", viewRes.ViewName);
            //Assert.IsFalse(viewRes.ViewBag.Create);
        }

        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            Mock<IGenericRepository<Subject>> mockR = new Mock<IGenericRepository<Subject>>();
            mockR.Setup(r => r.GetAll()).Returns(() => new Subject[]
                {
                    new Subject{ID = 1, Name = "N1"},
                    new Subject{ID = 2, Name = "N2"},
                    new Subject{ID = 3, Name = "N3"},
                    new Subject{ID = 4, Name = "N4"},
                    new Subject{ID = 5, Name = "N5"}
                }.AsQueryable());
            mock.Setup(u => u.Subjects).Returns(mockR.Object);
            SubjectController controller = new SubjectController(mock.Object);
            controller.PageSize = 3;
            SubjectsListViewModel result = (SubjectsListViewModel)controller.List(2).Model;

            Subject[] subjectArray = result.Subjects.ToArray();
            Assert.IsTrue(subjectArray.Length == 2);
            Assert.AreEqual(subjectArray[0].Name, "N4");
            Assert.AreEqual(subjectArray[1].Name, "N5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            Mock<IGenericRepository<Subject>> mockR = new Mock<IGenericRepository<Subject>>();
            mockR.Setup(r => r.GetAll()).Returns(() => new Subject[]
                {
                    new Subject{ID = 1, Name = "N1"},
                    new Subject{ID = 2, Name = "N2"},
                    new Subject{ID = 3, Name = "N3"},
                    new Subject{ID = 4, Name = "N4"},
                    new Subject{ID = 5, Name = "N5"}
                }.AsQueryable());
            mock.Setup(u => u.Subjects).Returns(mockR.Object);
            SubjectController controller = new SubjectController(mock.Object);
            controller.PageSize = 3;

            SubjectsListViewModel result = (SubjectsListViewModel)controller.List(2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void CreateSubject()
        {
            var res = controller.Create();
            Assert.IsInstanceOfType(res, typeof(ViewResult));
            var viewRes = res as ViewResult;

            Assert.IsInstanceOfType(viewRes.Model, typeof(Subject));
            //Assert.IsTrue(viewRes.ViewBag.Create);
        }
        [TestMethod]
        public void PostCreateSubject()
        {
            var res = controller.Create(subjects[0]);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            sMockRepository.Verify(r => r.Insert(subjects[0]));
            sMockRepository.Verify(r => r.Save());
        }
        [TestMethod]
        public void PostEditSubject()
        {
            var res = controller.Edit(1, subjects[0] as Subject);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            //mockRepository.Verify(r => r.Update(users[0]));
            sMockRepository.Verify(r => r.Save());
        }

        [TestMethod]
        public void DeleteSubject()
        {
            var res = controller.Delete(1);
            Assert.IsInstanceOfType(res, typeof(RedirectToRouteResult));
            var redirectRes = res as RedirectToRouteResult;

            Assert.IsNull(redirectRes.RouteValues["controller"]);
            Assert.AreEqual("List", redirectRes.RouteValues["action"]);

            sMockRepository.Verify(r => r.Delete(subjects[0]));
            sMockRepository.Verify(r => r.Save());
        }
    }
}
