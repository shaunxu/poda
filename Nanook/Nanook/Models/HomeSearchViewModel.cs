using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nanook.Shared;

namespace Nanook.Models
{
    public class HomeSearchViewModel : ViewModelBase
    {
        public IEnumerable<QuestionItem> Questions { get; set; }
        public string Keyword { get; set; }

        public class QuestionItem
        {
            public Guid ID { get; set; }
            public string Title { get; set; }
            public string Question { get; set; }
            public DateTime PostedOn { get; set; }
            public string PostedBy { get; set; }
            public Guid PostedByID { get; set; }
            public int AnswerCount { get; set; }
            public bool Answered { get; set; }
        }
    }
}