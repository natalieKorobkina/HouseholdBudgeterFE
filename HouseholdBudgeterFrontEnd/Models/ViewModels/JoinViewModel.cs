using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeterFrontEnd.Models.ViewModels
{
    public class JoinViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}