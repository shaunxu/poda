using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nanook.Models;

namespace Nanook.Controllers
{
    [Authorize]
    public class FeedbackController : NanookController
    {
        [HttpGet]
        public ActionResult Create(Guid questionId)
        {
            var memberId = Member.Data.ID;
            FeedbackCreateViewModel model;
            using (var poda = Poda.Factory.Create())
            {
                model = poda.Execute()
                    .ForPlainSQL("SELECT * FROM MemberQuestions WHERE ID = @ID")
                    .With("ID", questionId)
                    .FederationOn("MemberQuestions", "MemberID", memberId)
                    .AsEntities<FeedbackCreateViewModel>((reader) => new FeedbackCreateViewModel()
                    {
                        QuestionID = questionId,
                        MemberID = (Guid)reader["MemberID"],
                        QuestionTitle = (string)reader["Title"],
                        QuestionContent = (string)reader["Question"]
                    })
                    .FirstOrDefault();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(FeedbackCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var feedbackId=Guid.NewGuid();
                var feedbackMemberId=Member.Data.ID;
                using (var poda = Poda.Factory.Create())
                {
                    poda.Execute()
                        .ForPlainSQL("INSERT INTO MemberQuestionAnswers " +
                                     "(ID, MemberID, QuestionID, Answer, PostedByID) VALUES " +
                                     "(@ID, @MemberID, @QuestionID, @Answer, @PostedByID)")
                        .With("ID", feedbackId)
                        .With("MemberID", model.MemberID)
                        .With("QuestionID", model.QuestionID)
                        .With("Answer", model.Feedback)
                        .With("PostedByID", feedbackMemberId)
                        .FederationOn("MemberQuestionAnswers", "MemberID", model.MemberID)
                        .AsNothing();
                    poda.Commit();
                }
                ShowMessage("Thanks for your feedback.");
                return RedirectToAction("Details", "Question", new { questionId = model.QuestionID });
            }
            else
            {
                return Create(model.QuestionID);
            }
        }
    }
}
