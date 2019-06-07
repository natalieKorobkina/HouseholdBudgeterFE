﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeterFrontEnd.Models.ViewModels
{
    public class BankAccountViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public decimal Balance { get; set; }

        public int HouseholdId { get; set; }
    }
}