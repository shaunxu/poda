using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nanook.Shared;

namespace Nanook.Models
{
    public class QuestionMyOutstandingQuestionsViewModel : ViewModelBase
    {
        public IEnumerable<QuestionItem> Questions { get; set; }

        public class QuestionItem
        {
            public Guid ID { get; set; }
            public string Title { get; set; }
            public string Question { get; set; }
            public DateTime PostedOn { get; set; }
            public int AnswerCount { get; set; }
        }
    }
}