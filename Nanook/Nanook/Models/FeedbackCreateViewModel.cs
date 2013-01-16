using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nanook.Shared;
using System.ComponentModel.DataAnnotations;

namespace Nanook.Models
{
    public class FeedbackCreateViewModel : ViewModelBase
    {
        [Required]
        public string Feedback { get; set; }

        public Guid QuestionID { get; set; }
        public Guid MemberID { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionContent { get; set; }
    }
}