using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Domain.Entities;
using ITS.Domain.UnitOfWork;

namespace ITS.Models
{
    public class UsersListViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}