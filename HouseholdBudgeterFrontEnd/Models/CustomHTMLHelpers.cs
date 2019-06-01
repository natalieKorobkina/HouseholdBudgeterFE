using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HouseholdBudgeterFrontEnd.Models
{
    public static class CustomHTMLHelpers
    {
        public static MvcHtmlString BootstrapModalWindow(
            this HtmlHelper helper, string id, string title, string body)
        {
            return MvcHtmlString.Create($@"
<!-- Button trigger modal -->
<button type='button' class='btn btn-primary btn-lg' data-toggle='modal' data-target='#{id}'>
  Launch demo modal
</button>
<!-- Modal -->
<div class='modal fade' id='{id}' tabindex='-1' role='dialog' aria-labelledby='myModalLabel'>
  <div class='modal-dialog' role='document'>
    <div class='modal-content'>
      <div class='modal-header'>
        <button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>
        <h4 class='modal-title' id='myModalLabel'>{title}</h4>
      </div>
      <div class='modal-body'>
        {body}
      </div>
      <div class='modal-footer'>
        <button type='button' class='btn btn-default' data-dismiss='modal'>Close</button>
        <button type='button' class='btn btn-primary'>Save changes</button>
      </div>
    </div>
  </div>
</div>");
        }
    }
}