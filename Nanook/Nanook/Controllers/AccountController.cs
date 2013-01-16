using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Nanook.Models;
using Nanook.Shared;
using Nanook.Entities;
using Ethos.Infrastructure.Security;

namespace Nanook.Controllers
{
    public class AccountController : NanookController
    {
        [HttpGet]
        public ActionResult Register()
        {
            var model = new AccountRegisterViewModel();
            using (var poda = Poda.Factory.Create())
            {
                var countries = poda.Execute()
                    .ForPlainSQL("SELECT * FROM Countries ORDER BY Country ASC")
                    .ReferenceOn("Countries")
                    .AsEntities<KeyValuePair<Guid, string>>(new LookupEntityConverter("ID", "Country"));
                model.Countries = new SelectList(countries, "Key", "Value");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Register(AccountRegisterViewModel model)
        {
            // server side validation
            if (ModelState.IsValid)
            {
                using (var poda = Poda.Factory.Create())
                {
                    var existingMemberCount = poda.Execute()
                        .ForPlainSQL("SELECT COUNT(ID) FROM Members WHERE Email = @Email")
                        .With("Email", model.Email)
                        .FederationOnAll()
                        .As<int>();
                    if (existingMemberCount > 0)
                    {
                        ModelState.AddModelError("Email", "This email address had been used.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                // save the new member
                using (var poda = Poda.Factory.Create())
                {
                    var memberId = Guid.NewGuid();
                    poda.Execute()
                        .ForPlainSQL("INSERT INTO Members (ID, Email, Password, CountryID) VALUES (@ID, @Email, @Password, @CountryID)")
                        .With("ID", memberId)
                        .With("Email", model.Email)
                        .With("Password", model.Password)
                        .With("CountryID", model.Country)
                        .FederationOn("Members", "ID", memberId)
                        .AsNothing();
                    poda.Commit();
                }
                // log in as this member
                var actionResult = LogOn(new AccountLogOnViewModel()
                    {
                        Email = model.Email,
                        Password = model.Password,
                        ReturnURL = string.Empty
                    });

                ShowMessage("Your account had been registered successful and logged in the system now.");
                return actionResult;
            }
            else
            {
                // display error message and back
                return Register();
            }
        }

        [HttpGet]
        public ActionResult LogOn(string returnUrl)
        {
            return View(new AccountLogOnViewModel() { ReturnURL = returnUrl });
        }

        [HttpPost]
        public ActionResult LogOn(AccountLogOnViewModel model)
        {
            IEnumerable<Member> members = null;
            Member member = null;
            // server side validation
            if (ModelState.IsValid)
            {
                using (var poda = Poda.Factory.Create())
                {
                    members = poda.Execute()
                        .ForPlainSQL("SELECT * FROM Members WHERE Email = @Email AND Password = @Password")
                        .With("Email", model.Email)
                        .With("Password", model.Password)
                        .FederationOnAll()
                        .AsEntities<Member>(new MemberEntityConverter());
                    if (members.Count() <= 0)
                    {
                        ModelState.AddModelError("Email", "Invalid email or password.");
                    }
                    else if (members.Count() > 1)
                    {
                        ModelState.AddModelError("Email", "There are more than one member with your email and password please contact the administrator.");
                    }
                    else
                    {
                        member = members.First();
                    }
                }
            }
            // login
            if (ModelState.IsValid)
            {
                var auth = new SimplePrincipalAuthenticationHelper();
                var cookie = auth.CreateAuthenticationCookie(model.RememberMe, member, m => m.Email, DateTime.Now);
                Response.Cookies.Add(cookie);
                if (string.IsNullOrWhiteSpace(model.ReturnURL))
                {
                    return RedirectToAction("Index", "Home", null);
                }
                else
                {
                    return Redirect(model.ReturnURL);
                }
            }
            else
            {
                return LogOn(model.ReturnURL);
            }
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", null);
        }
    }
}