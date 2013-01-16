using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nanook.Entities;
using Nanook.Models;
using Nanook.Shared;

namespace Nanook.Controllers
{
    public class QuestionController : NanookController
    {
        [Authorize]
        [HttpGet]
        public ActionResult MyOutstandingQuestions()
        {
            var model = new QuestionMyOutstandingQuestionsViewModel();
            var memberId = Member.Data.ID;
            using (var poda = Poda.Factory.Create())
            {
                model.Questions = poda.Execute()
                    .ForPlainSQL("SELECT MemberQuestions.ID, Title, Question, MemberQuestions.PostedOn, COUNT(MemberQuestionAnswers.ID) AS AnswerCount " +
                                 "FROM MemberQuestions " +
                                 "LEFT JOIN MemberQuestionAnswers ON MemberQuestions.ID = MemberQuestionAnswers.QuestionID " +
                                 "WHERE MemberQuestions.MemberID = @MemberID AND MemberQuestions.AnswerID IS NULL " +
                                 "GROUP BY MemberQuestions.ID, Title, Question, MemberQuestions.PostedOn " +
                                 "ORDER BY MemberQuestions.PostedOn DESC ")
                    .With("MemberID", memberId)
                    .FederationOn("MemberQuestions", "MemberID", memberId)
                    .AsEntities<QuestionMyOutstandingQuestionsViewModel.QuestionItem>(reader => new QuestionMyOutstandingQuestionsViewModel.QuestionItem()
                    {
                        ID = (Guid)reader["ID"],
                        Title = (string)reader["Title"],
                        Question = (string)reader["Question"],
                        PostedOn = (DateTime)reader["PostedOn"],
                        AnswerCount = (int)reader["AnswerCount"]
                    });
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult MyQuestions()
        {
            var model = new QuestionMyQuestionsViewModel();
            var memberId = Member.Data.ID;
            using (var poda = Poda.Factory.Create())
            {
                model.Questions = poda.Execute()
                    .ForPlainSQL("SELECT MemberQuestions.ID, Title, Question, MemberQuestions.PostedOn, COUNT(MemberQuestionAnswers.ID) AS AnswerCount, MemberQuestions.AnswerID " +
                                 "FROM MemberQuestions " +
                                 "LEFT JOIN MemberQuestionAnswers ON MemberQuestions.ID = MemberQuestionAnswers.QuestionID " +
                                 "WHERE MemberQuestions.MemberID = @MemberID " +
                                 "GROUP BY MemberQuestions.ID, Title, Question, MemberQuestions.PostedOn, MemberQuestions.AnswerID " +
                                 "ORDER BY MemberQuestions.PostedOn DESC ")
                    .With("MemberID", memberId)
                    .FederationOn("MemberQuestions", "MemberID", memberId)
                    .AsEntities<QuestionMyQuestionsViewModel.QuestionItem>(reader => new QuestionMyQuestionsViewModel.QuestionItem()
                    {
                        ID = (Guid)reader["ID"],
                        Title = (string)reader["Title"],
                        Question = (string)reader["Question"],
                        PostedOn = (DateTime)reader["PostedOn"],
                        AnswerCount = (int)reader["AnswerCount"],
                        HasValidAnswer = (reader["AnswerID"] != DBNull.Value)
                    });
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(Guid questionId)
        {
            var memberId = Member.Data.ID;
            QuestionEditViewModel model = null;

            using (var poda = Poda.Factory.Create())
            {
                // load the question
                model = poda.Execute()
                    .ForPlainSQL("SELECT * FROM MemberQuestions WHERE ID = @ID")
                    .With("ID", questionId)
                    .FederationOn("MemberQuestions", "MemberID", memberId)
                    .AsEntities<QuestionEditViewModel>((reader) => new QuestionEditViewModel()
                    {
                        ID = questionId,
                        Question = (string)reader["Question"],
                        Title = (string)reader["Title"]
                    })
                    .FirstOrDefault();
                // load the tags
                var tags = poda.Execute()
                    .ForPlainSQL("SELECT Tag FROM MemberQuestionTags " +
                                 "LEFT JOIN Tags ON Tags.ID = MemberQuestionTags.TagID " +
                                 "WHERE QuestionID = @QuestionID " +
                                 "ORDER BY ReferenceCount DESC ")
                    .With("QuestionID", questionId)
                    .FederationOn("MemberQuestionTags", "MemberID", memberId)
                    .AsEntities<string>((reader) => (string)reader["Tag"]);
                model.Tags = tags.Combine(',');
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(QuestionEditViewModel model)
        {
            var memberId = Member.Data.ID;
            if (ModelState.IsValid)
            {
                using (var poda = Poda.Factory.Create())
                {
                    poda.Execute()
                        .ForPlainSQL("UPDATE MemberQuestions SET Title = @Title, Question = @Question " +
                                     "WHERE ID = @ID ")
                        .With("Title", model.Title)
                        .With("Question", model.Question)
                        .With("ID", model.ID)
                        .FederationOn("MemberQuestions", "MemberID", memberId)
                        .AsNothing();
                    poda.Commit();
                }
                ShowMessage("Your question had been changed successful.");
                return RedirectToAction("MyOutstandingQuestions");
            }
            else
            {
                return Edit(model.ID);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View(new QuestionCreateViewModel());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(QuestionCreateViewModel model)
        {
            var memberId = Member.Data.ID;
            var questionId = Guid.NewGuid();
            var tagsSpecified = model.Tags.Split(',')
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.ToLower())
                .ToList();
            using (var poda = Poda.Factory.Create())
            {
                // retrieve the existing tag list
                var tagsExisting = poda.Execute()
                    .ForPlainSQL("SELECT * FROM Tags")
                    .ReferenceOn("Tags")
                    .AsEntities<Tag>((reader) => new Tag()
                    {
                        ID = (Guid)reader["ID"],
                        Value = ((string)reader["Tag"]).ToLower(),
                        ReferneceCount = (long)reader["ReferenceCount"]
                    });
                // save new question
                poda.Execute()
                    .ForPlainSQL("INSERT INTO MemberQuestions (ID, MemberID, Title, Question) VALUES (@ID, @MemberID, @Title, @Question)")
                    .With("ID", questionId)
                    .With("MemberID", memberId)
                    .With("Title", model.Title)
                    .With("Question", model.Question)
                    .FederationOn("MemberQuestions", "MemberID", memberId)
                    .AsNothing();
                // save the tags and relationship
                Guid tagId;
                foreach (var tagSpecified in tagsSpecified)
                {
                    var tag = tagsExisting.Where(t => t.Value == tagSpecified).FirstOrDefault();
                    if (tag == null)
                    {
                        // save the new tag if it's no exist in tag database table
                        tagId = Guid.NewGuid();
                        poda.Execute()
                            .ForPlainSQL("INSERT INTO Tags (ID, Tag) VALUES (@ID, @Tag)")
                            .With("ID", tagId)
                            .With("Tag", tagSpecified)
                            .FederationOnAll()
                            .AsNothing();
                    }
                    else
                    {
                        tagId = tag.ID;
                        // update the reference count of the existing tag
                        poda.Execute()
                            .ForPlainSQL("UPDATE Tags SET ReferenceCount = @ReferenceCount WHERE ID = @ID")
                            .With("ReferenceCount", tag.ReferneceCount + 1)
                            .With("ID", tagId)
                            .FederationOnAll()
                            .AsNothing();
                    }
                    // create the question tag relationship
                    var questionTagId = Guid.NewGuid();
                    poda.Execute()
                        .ForPlainSQL("INSERT INTO MemberQuestionTags (ID, MemberID, QuestionID, TagID) VALUES (@ID, @MemberID, @QuestionID, @TagID)")
                        .With("ID", questionTagId)
                        .With("MemberID", memberId)
                        .With("QuestionID", questionId)
                        .With("TagID", tagId)
                        .FederationOn("MemberQuestionTags", "MemberID", memberId)
                        .AsNothing();
                    // save changes
                    poda.Commit();
                }
                // back to my outstanding question list
                ShowMessage("You question had been submitted please wait for the answers.");
                return RedirectToAction("MyOutstandingQuestions");
            }
        }

        [HttpGet]
        public ActionResult Details(Guid questionId)
        {
            var memberId = ((Request.IsAuthenticated && Member != null && Member.Data != null) ? Member.Data.ID : Guid.Empty);
            var model = new QuestionDetailsViewModel();

            using (var poda = Poda.Factory.Create())
            {
                // load the question
                var question = poda.Execute()
                    .ForPlainSQL("SELECT MemberQuestions.*, Members.Email " +
                                 "FROM MemberQuestions " +
                                 "LEFT JOIN Members ON Members.ID = MemberQuestions.MemberID " +
                                 "WHERE MemberQuestions.ID = @ID ")
                    .With("ID", questionId)
                    .FederationOn("MemberQuestions", "MemberID", memberId)
                    .AsEntities<QuestionDetailsViewModel.QuestionItem>((reader) => new QuestionDetailsViewModel.QuestionItem()
                    {
                        ID = questionId,
                        Title = (string)reader["Title"],
                        Content = (string)reader["Question"],
                        PostedOn = (DateTime)reader["PostedOn"],
                        PostedByID = (Guid)reader["MemberID"],
                        PostedBy = (string)reader["Email"],
                        IsMyQuestion = ((Guid)reader["MemberID"] == memberId),
                        AnswerID = reader["AnswerID"] == DBNull.Value ? new Guid?() : (Guid)reader["AnswerID"]
                    })
                    .FirstOrDefault();
                // load the tags (separated by comma)
                var tags = poda.Execute()
                    .ForPlainSQL("SELECT Tag FROM MemberQuestionTags " +
                                 "LEFT JOIN Tags ON Tags.ID = MemberQuestionTags.TagID " +
                                 "WHERE QuestionID = @QuestionID " +
                                 "ORDER BY ReferenceCount DESC ")
                    .With("QuestionID", questionId)
                    .FederationOn("MemberQuestionTags", "MemberID", memberId)
                    .AsEntities<string>((reader) => (string)reader["Tag"]);
                question.Tags = tags.Combine(',');
                model.Question = question;
                // load the feedbacks of this question
                var feedbacks = poda.Execute()
                    .ForPlainSQL("SELECT MemberQuestionAnswers.ID, Answer, PostedOn, Members.Email, Members.ID AS PostedByID FROM MemberQuestionAnswers " +
                                 "LEFT JOIN Members ON Members.ID = MemberQuestionAnswers.PostedByID " +
                                 "WHERE MemberQuestionAnswers.QuestionID = @QuestionID ")
                    .With("QuestionID", questionId)
                    .FederationOn("MemberQuestionAnswers", "MemberID", memberId)
                    .AsEntities<QuestionDetailsViewModel.FeedbackItem>((reader) => new QuestionDetailsViewModel.FeedbackItem()
                    {
                        ID = (Guid)reader["ID"],
                        Feedback = (string)reader["Answer"],
                        PostedOn = (DateTime)reader["PostedOn"],
                        PostedBy = (string)reader["Email"],
                        PostedByID = (Guid)reader["PostedByID"],
                        IsMyFeedback = ((Guid)reader["PostedByID"] == memberId),
                        IsAnswer = (question.AnswerID.HasValue && question.AnswerID.Value == (Guid)reader["ID"])
                    });
                model.Feedbacks = feedbacks;
            }
            return View(model);
        }

        
        // this action should navigate to another page and use POST or use Ajax and PUT 
        // HTTP method to update the question information rather than GET 
        // but since this is just a demo i utilize a normal GET linkage to implement
        [Authorize]
        [HttpGet]
        public ActionResult MarkAnswer(Guid feedbackId, Guid questionId, Guid postedById)
        {
            Guid currentMemberId = Member.Data.ID;
            using (var poda = Poda.Factory.Create())
            {
                poda.Execute()
                    .ForPlainSQL("UPDATE MemberQuestions SET AnswerID = @AnswerID WHERE ID = @ID")
                    .With("AnswerID", feedbackId)
                    .With("ID", questionId)
                    .FederationOn("MemberQuestions", "MemberID", postedById)
                    .AsNothing();
                poda.Commit();
            }

            ShowMessage("The feedback had been marked as answer.");
            return RedirectToAction("Details", new { questionId = questionId });
        }

        // this action should navigate to another page and use POST or use Ajax and PUT 
        // HTTP method to update the question information rather than GET 
        // but since this is just a demo i utilize a normal GET linkage to implement
        [Authorize]
        [HttpGet]
        public ActionResult UnmarkAnswer(Guid questionId, Guid postedById)
        {
            Guid currentMemberId = Member.Data.ID;
            using (var poda = Poda.Factory.Create())
            {
                poda.Execute()
                    .ForPlainSQL("UPDATE MemberQuestions SET AnswerID = NULL WHERE ID = @ID")
                    .With("ID", questionId)
                    .FederationOn("MemberQuestions", "MemberID", postedById)
                    .AsNothing();
                poda.Commit();
            }

            ShowMessage("The question had been marked as unresolved.");
            return RedirectToAction("Details", new { questionId = questionId });
        }

    }
}
