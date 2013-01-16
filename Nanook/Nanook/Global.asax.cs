using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.SqlClient;
using Poda.Configuration.ConfigurationFileProvider;
using System.Web.Security;
using Ethos.Infrastructure.Security;
using Nanook.Entities;

namespace Nanook
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        public static void InitializePoda()
        {
            Poda.Factory.Config(new ConfigurationFileProvider(), (cs) => new SqlConnection(cs));
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitializePoda();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported)
            {
                HttpApplication app = (HttpApplication)sender;
                if (app.Request.IsAuthenticated && app.User.Identity is FormsIdentity)
                {
                    string cookieName = FormsAuthentication.FormsCookieName;
                    HttpCookie cookie = Context.Request.Cookies[cookieName];

                    if (cookie == null) return;

                    FormsAuthenticationTicket ticket = null;
                    try
                    {
                        ticket = FormsAuthentication.Decrypt(cookie.Value);
                    }
                    catch
                    {
                        return;
                    }

                    if (ticket == null) return;

                    FormsIdentity identity = new FormsIdentity(ticket);
                    ISimplePrincipalAuthenticationHelper helper = new SimplePrincipalAuthenticationHelper();
                    // for now there's no role in the system so the IsInRole we just return TRUE for all users
                    SimplePrincipal<Member> principal = helper.RetrievePrincipalFromCookie<Member>(cookie, (m, role) => true);

                    Context.User = principal;
                }
            }
            else
            {
                throw new HttpException("Cookies is not supported for this application.");
            }
        }
    }
}