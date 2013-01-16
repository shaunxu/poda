using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nanook.Models;
using System.Text;
using Poda.Shared;

namespace Nanook.Controllers
{
    public class HomeController : NanookController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new HomeIndexViewModel();
            using (var poda = Poda.Factory.Create())
            {
                model.Questions = poda.Execute()
                    .ForPlainSQL("SELECT MemberQuestions.ID, MemberQuestions.Title, MemberQuestions.Question, MemberQuestions.PostedOn, " +
                                 "Members.Email, MemberQuestions.MemberID, COUNT(MemberQuestionAnswers.ID) AS AnswerCount " +
                                 "FROM MemberQuestions " +
                                 "LEFT JOIN Members ON Members.ID = MemberQuestions.MemberID " +
                                 "LEFT JOIN MemberQuestionAnswers ON MemberQuestionAnswers.QuestionID = MemberQuestions.ID " +
                                 "WHERE MemberQuestions.AnswerID IS NULL " +
                                 "GROUP BY MemberQuestions.ID, MemberQuestions.Title, MemberQuestions.Question, MemberQuestions.PostedOn, " +
                                 "Members.Email, MemberQuestions.MemberID")
                    .FederationOnAll()
                    .AsEntities((reader) => new HomeIndexViewModel.QuestionItem()
                    {
                        ID = (Guid)reader["ID"],
                        Title = (string)reader["Title"],
                        Question = (string)reader["Question"],
                        PostedOn = (DateTime)reader["PostedOn"],
                        PostedBy = (string)reader["Email"],
                        PostedByID = (Guid)reader["MemberID"],
                        AnswerCount = (int)reader["AnswerCount"]
                    })
                    .OrderByDescending(q => q.PostedOn)
                    .ToList();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ShowMessage("Please input what you want to search.");
                if (Request.UrlReferrer == null)
                {
                    return RedirectToAction("Index", "Home", null);
                }
                else
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            else
            {
                var model = new HomeSearchViewModel()
                {
                    Keyword = keyword
                };
                // split the key word by space
                var keywords = keyword.Split(' ').Where(k => !string.IsNullOrWhiteSpace(k)).ToList();
                // build the sql text
                var sql = new StringBuilder();
                sql.Append("SELECT MemberQuestions.ID, MemberQuestions.Title, MemberQuestions.Question, MemberQuestions.PostedOn, " +
                           "Members.Email, MemberQuestions.MemberID, COUNT(MemberQuestionAnswers.ID) AS AnswerCount, MemberQuestions.AnswerID " +
                           "FROM MemberQuestions " +
                           "LEFT JOIN Members ON Members.ID = MemberQuestions.MemberID " +
                           "LEFT JOIN MemberQuestionAnswers ON MemberQuestionAnswers.QuestionID = MemberQuestions.ID " +
                           "WHERE 1 = 1 ");
                for (int i = 0; i < keywords.Count; i++)
                {
                    sql.AppendFormat("AND (MemberQuestions.Title LIKE '%' + @keyword{0} + '%' OR MemberQuestions.Question LIKE '%' + @keyword{0} + '%') ", i);
                }
                sql.Append("GROUP BY MemberQuestions.ID, MemberQuestions.Title, MemberQuestions.Question, MemberQuestions.PostedOn, " +
                           "Members.Email, MemberQuestions.MemberID, MemberQuestions.AnswerID ");
                // execute
                using (var poda = Poda.Factory.Create())
                {
                    var command = poda.Execute().ForPlainSQL(sql.ToString()) as ICommandAfterWith;
                    for (int i = 0; i < keywords.Count; i++)
                    {
                        command = command.With(string.Format("keyword{0}", i), keywords[i]);
                    }
                    model.Questions = command.FederationOnAll()
                        .AsEntities((reader) => new HomeSearchViewModel.QuestionItem()
                        {
                            ID = (Guid)reader["ID"],
                            Title = (string)reader["Title"],
                            Question = (string)reader["Question"],
                            PostedOn = (DateTime)reader["PostedOn"],
                            PostedBy = (string)reader["Email"],
                            PostedByID = (Guid)reader["MemberID"],
                            AnswerCount = (int)reader["AnswerCount"],
                            Answered = (reader["AnswerID"] != DBNull.Value)
                        })
                        .OrderByDescending(q => q.PostedOn)
                        .ToList();
                }
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}
