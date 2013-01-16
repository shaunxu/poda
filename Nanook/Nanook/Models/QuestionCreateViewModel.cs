using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nanook.Shared;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nanook.Models
{
    public class QuestionCreateViewModel : ViewModelBase
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Tags { get; set; }
    }
}