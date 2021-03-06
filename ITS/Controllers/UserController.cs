﻿using ITS.Domain.UnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork;
using ITS.Models;
using System.Web.Helpers;
using ITS.Infrastructure;

namespace ITS.Controllers
{
	[Auth(UserRole.Admin)]
    public class UserController : Controller
    {
        private IUnitOfWork unitOfWork;

        public int PageSize = 10;

        public UserController(IUnitOfWork uow)
        {
            this.unitOfWork = uow;
        }

        //
        // GET: /User/
		
        public ViewResult List(int page = 1)
        {
            UsersListViewModel model = new UsersListViewModel
            {
                Users = unitOfWork.Users.GetAll()
                .OrderBy(o => o.LastName).ToList()
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = unitOfWork.Users.GetAll().Count()
                }
            };

            return View(model);
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /User/Create

        public ViewResult Create()
        {
            var groups = unitOfWork.Groups.GetAll().Select(g => new SelectListItem()
            {
                Text = g.Name,
                Value = g.ID.ToString()

            });
			/*
            var roles = new List<SelectListItem>();
            roles.Add(new SelectListItem()
                {
                    Text = "Student",
                    Value = "0"
                });
            if(CurrentUser().Role == UserRole.Admin)
            {
                roles.Add(new SelectListItem()
                {
                    Text = "Teacher",
                    Value = "1"
                });
                roles.Add(new SelectListItem()
                {
                    Text = "Admin",
                    Value = "2"
                });
            }
            ViewBag.Roles = roles;
			 * */
			ViewBag.AllowChooseRole = CurrentUser().IsAdmin;
            ViewBag.Create = true;
            ViewBag.Groups = groups;
			return View("Edit", new User() { IsStudent = true });
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                unitOfWork.Users.Insert(user);
                unitOfWork.Save();

                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.Error = true;
                return View(user);
            }
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            User user = unitOfWork.Users.GetByID(id);
            var groups = unitOfWork.Groups.GetAll().Select(g => new SelectListItem()
            {
                Text = g.Name,
                Value = g.ID.ToString()

            });
			/*
            var roles = new List<SelectListItem>();
            roles.Add(new SelectListItem()
            {
                Text = "Student",
                Value = "0",
                Selected = user.Role == UserRole.Student
            });
            if (CurrentUser().Role == UserRole.Admin)
            {
                roles.Add(new SelectListItem()
                {
                    Text = "Teacher",
                    Value = "1",
                    Selected = user.Role == UserRole.Teacher
                });
                roles.Add(new SelectListItem()
                {
                    Text = "Admin",
                    Value = "2",
                    Selected = user.Role == UserRole.Admin
                });
            }
            ViewBag.Roles = roles;
			 * */
			ViewBag.AllowChooseRole = CurrentUser().IsAdmin;
            ViewBag.Create = false;
            ViewBag.Groups = groups;

            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, User user)
        {
            if(ModelState.IsValid)
            {
                SaveUser(user);
                TempData["message"] = string.Format("User {0} {1} has been saved!", user.FirstName, user.LastName);
                return RedirectToAction("List");
            }
            else
            {
                var groups = unitOfWork.Groups.GetAll().Select(g => new SelectListItem()
                {
                    Text = g.Name,
                    Value = g.ID.ToString()

                });
				/*
                var roles = new List<SelectListItem>();
                roles.Add(new SelectListItem()
                {
                    Text = "Student",
                    Value = "0",
                    Selected = user.Role == UserRole.Student
                });
                if (CurrentUser().Role == UserRole.Admin)
                {
                    roles.Add(new SelectListItem()
                    {
                        Text = "Teacher",
                        Value = "1",
                        Selected = user.Role == UserRole.Teacher
                    });
                    roles.Add(new SelectListItem()
                    {
                        Text = "Admin",
                        Value = "2",
                        Selected = user.Role == UserRole.Admin
                    });
                }
                ViewBag.Roles = roles;
				 * */
				ViewBag.AllowChooseRole = CurrentUser().IsAdmin;
                ViewBag.Groups = groups;
                ViewBag.Create = user.ID == 0;
                return View(user);
            }
            //try
            //{
            //    unitOfWork.Users.Update(user);
            //    unitOfWork.Save();

            //    return RedirectToAction("List");
            //}
            //catch
            //{
            //    ViewBag.Error = true;
            //    return View(user);
            //}
        }

        //
        // GET: /User/Delete/5

        //public ActionResult Delete(int id)
        //{
        //    var user = unitOfWork.Users.GetByID(id);
        //    return View(user);
        //}

        public User DeleteUser(int id)
        {
            User dbEntry = unitOfWork.Users.GetByID(id);
            if (dbEntry != null)
            {
                unitOfWork.Users.Delete(dbEntry);
                unitOfWork.Save();
            }

            return dbEntry;
        }

        //
        // POST: /User/Delete/5

        //[HttpPost] -??? Not working!
        public ActionResult Delete(int id)
        {
            User deletedUser = DeleteUser(id);
            if(deletedUser != null)
            {
                TempData["message"] = string.Format("User {0} {1} was deleted!", deletedUser.FirstName, deletedUser.LastName);
            }

            return RedirectToAction("List");
        }

        public void SaveUser(User user)
        {
            if (user.Password != null)
            {
                user.Password = Crypto.HashPassword(user.Password);
            }

            if(user.ID == 0)
            {
                unitOfWork.Users.Insert(user);
            }
            else
            {
                User dbEntry = unitOfWork.Users.GetByID(user.ID);

                if(dbEntry != null)
                {
                    dbEntry.LastName = user.LastName;
                    dbEntry.FirstName = user.FirstName;
                    dbEntry.Login = user.Login;

                    if (user.Password != null)
                    {
                        dbEntry.Password = user.Password;
                    }
                    if (user.ID != CurrentUser().ID)
                    {
						dbEntry.IsAdmin = user.IsAdmin;
						dbEntry.IsTeacher = user.IsTeacher;
						dbEntry.IsStudent = user.IsStudent;
                    }
                }
                //unitOfWork.Users.Update(user);
            }
            unitOfWork.Save();
        }

        public ActionResult AddGroup(int groupId, int userId)
        {
            var user1 = unitOfWork.Users.GetByID(userId);
            var group1 = unitOfWork.Groups.GetByID(groupId);

            group1.Users.Add(user1);
            unitOfWork.Save();

            return RedirectToAction("Edit", new { id =userId});
        }

        public ActionResult RemoveGroup(int groupId, int userId)
        {
            var user1 = unitOfWork.Users.GetByID(userId);
            var group1 = unitOfWork.Groups.GetByID(groupId);

            user1.Groups.Remove(group1);
            unitOfWork.Save();

            return RedirectToAction("Edit", new { id = userId });
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
