using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HouseholdBudgeterFrontEnd.Models.ActionFilters
{
    public class CheckAutorization : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["HBFrontEnd"];
            var httpClient = new HttpClient();

            if (cookie == null)
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Account" }, { "action", "Login" } });
            else
            {
                var token = cookie.Value;
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                filterContext.HttpContext.Items["httpClient"] = httpClient;
            }
        }
    }
}