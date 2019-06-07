using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeterFrontEnd.Models.ViewModels
{
    public class InviteViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string HouseholdName { get; set; }
    }
}