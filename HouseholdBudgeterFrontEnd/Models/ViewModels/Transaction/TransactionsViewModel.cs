using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeterFrontEnd.Models.ViewModels
{
    public class TransactionsViewModel
    {
        public string BankAccountName { get; set; }
        public int BankAccountId { get; set; }

        public List<TransactionViewModel> Transactions { get; set; }

        public TransactionsViewModel()
        {
            Transactions = new List<TransactionViewModel>();
        }
    }
}