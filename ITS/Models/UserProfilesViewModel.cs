using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ITS.Domain.Entities
{
    public class UserProfilesViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Editable(true), Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Editable(false), Display(Name = "Last Name")]
        public string LastName { get; set; }

        [ReadOnly(true), Display(Name = "Login")]
        public string Login { get; set; }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

