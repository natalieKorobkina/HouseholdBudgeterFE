using System;
using System.ComponentModel.DataAnnotations;

namespace HouseholdBudgeterFrontEnd.Models
{
    public class CustomDateRangeAttribute : ValidationAttribute
    //: RangeAttribute
    //
    {
    //    public CustomDateRangeAttribute()
    //: base(typeof(DateTime),
    //        DateTime.Now.AddYears(-6).ToShortDateString(),
    //        DateTime.Now.ToShortDateString())
    //    { }
    //}
        //public override bool IsValid(object value)// Return a boolean value: true == IsValid, false != IsValid
        //{
        //    DateTime d = Convert.ToDateTime(value);
        //    return d >= DateTime.Now || d < Convert.ToDateTime("1/1/1919"); //Dates Greater than or equal to today are valid (true)

        //}
        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    DateTime _dateJoin = Convert.ToDateTime(value);
        //    if (_dateJoin >= DateTime.Now)
        //    {
        //        return ValidationResult.Success;
        //    }
        //    else
        //    {
        //        return new ValidationResult(ErrorMessage);
        //    }
        //}
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            value = (DateTime)value;
            // This assumes inclusivity, i.e. exactly six years ago is okay
            if (DateTime.Now.AddYears(-6).CompareTo(value) <= 0 && DateTime.Now.CompareTo(value) >= 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Date must be within the last six years!");
            }
        }
    }
}