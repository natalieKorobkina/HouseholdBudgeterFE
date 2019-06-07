using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeterFrontEnd.Models.ViewModels
{
    public class HouseholdDetailsViewModel
    {
        public string BankAccountName { get; set; }
        public decimal BankAccountBalance { get; set; }
        public List<SumCategory> Categories { get; set; }
    }

    public class SumCategory
    {
        public string CategoryName { get; set; }
        public decimal CategoryBalance { get; set; }
    }
}