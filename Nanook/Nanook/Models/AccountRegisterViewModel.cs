using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nanook.Shared;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nanook.Models
{
    public class AccountRegisterViewModel : ViewModelBase
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string RepeatPassword { get; set; }

        [Required]
        public Guid Country { get; set; }

        [Display(Name = "Country")]
        public SelectList Countries { get; set; }
    }
}