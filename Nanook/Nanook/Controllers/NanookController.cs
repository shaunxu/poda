using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ethos.Modules.WebMessager;
using Nanook.Entities;
using Ethos.Infrastructure.Security;

namespace Nanook.Controllers
{
    public abstract class NanookController : Controller
    {
        private IMessageProvider _errorMessage;
        private IMessageProvider _informationMessage;

        protected SimplePrincipal<Member> Member
        {
            get
            {
                return User as SimplePrincipal<Member>;
            }
        }

        public NanookController(IMessageProvider errorMessage, IMessageProvider informationMessage)
        {
            _errorMessage = errorMessage;
            _informationMessage = informationMessage;
        }

        public NanookController()
        {
            _errorMessage = new ModelStateMessageProvider(ModelState);
            _informationMessage = new TempDataMessagerProvider(TempData, "Nanook.TempDataMessager");
        }

        protected void ShowMessage(string message)
        {
            _informationMessage.AddMessage("_FORM", message);
        }
    }
}