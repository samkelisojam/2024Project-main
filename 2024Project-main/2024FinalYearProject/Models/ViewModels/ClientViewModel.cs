﻿namespace _2024FinalYearProject.Models.ViewModels
{
    public class BankAccountViewModel
    {
        public IEnumerable<BankAccount> BankAccount { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
    public class CashSentViewModel
    {
        public int BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal AvailableBalance { get; set; }  // Add this property
    }
    public class BankAccountViewModela
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string BankAccountType { get; set; }
    }


}