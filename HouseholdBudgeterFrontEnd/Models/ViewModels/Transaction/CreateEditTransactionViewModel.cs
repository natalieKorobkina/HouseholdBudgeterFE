using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HouseholdBudgeterFrontEnd.Models.ViewModels
{
    public class CreateEditTransactionViewModel : IValidatableObject
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Ammount { get; set; }
        [Required]
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        //[Range(typeof(DateTime), "1/1/1919", "1/1/2019")]
        //ErrorMessage = "Value for {0} must be between {1} and {2}")]
        //[CustomDateRange]
        public DateTime TransactionDate { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        public int BankAccountId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TransactionDate > DateTime.Now)
            {
                yield return
                  new ValidationResult(errorMessage: "TransactionDate cannot be greater than today",
                                       memberNames: new[] { "TransactionDate" });
            }
        }
    }

    //public class CustomDateRangeAttribute : RangeAttribute
    //{
    //    public CustomDateRangeAttribute() : base(typeof(DateTime), "1/1/1919", DateTime.Now.ToString())
    //    { }
    //}
}