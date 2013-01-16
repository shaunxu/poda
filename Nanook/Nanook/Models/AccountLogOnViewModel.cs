using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nanook.Shared;
using System.ComponentModel.DataAnnotations;

namespace Nanook.Models
{
    public class AccountLogOnViewModel : ViewModelBase
    {
        public string ReturnURL { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}