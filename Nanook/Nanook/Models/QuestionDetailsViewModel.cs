using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nanook.Shared;

namespace Nanook.Models
{
    public class QuestionDetailsViewModel : ViewModelBase
    {
        public QuestionItem Question { get; set; }
        public IEnumerable<FeedbackItem> Feedbacks { get; set; }

        public class QuestionItem
        {
            public Guid ID { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public string Tags { get; set; }
            public string PostedBy { get; set; }
            public Guid PostedByID { get; set; }
            public bool IsMyQuestion { get; set; }
            public DateTime PostedOn { get; set; }
            public Guid? AnswerID { get; set; }
        }
        
        public class FeedbackItem
        {
            public Guid ID { get; set; }
            public string Feedback { get; set; }
            public DateTime PostedOn { get; set; }
            public string PostedBy { get; set; }
            public Guid PostedByID { get; set; }
            public bool IsMyFeedback { get; set; }
            public bool IsAnswer { get; set; }
        }
    }
}