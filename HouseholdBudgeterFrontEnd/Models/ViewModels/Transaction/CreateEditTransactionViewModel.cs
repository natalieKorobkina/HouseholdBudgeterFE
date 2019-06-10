using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HouseholdBudgeterFrontEnd.Models.ViewModels
{
    public class CreateEditTransactionViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Ammount { get; set; }
        [Required]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Required]
        public int? CategoryId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        public int BankAccountId { get; set; }
    }
}