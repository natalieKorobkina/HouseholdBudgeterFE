using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HouseholdBudgeterFrontEnd.Models.ActionFilters
{
    public class CheckModelState : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)

                filterContext.Result = new ViewResult
                {
                    ViewName = filterContext.ActionDescriptor.ActionName,
                    ViewData = filterContext.Controller.ViewData,
                    TempData = filterContext.Controller.TempData
                };
        }

    }
}