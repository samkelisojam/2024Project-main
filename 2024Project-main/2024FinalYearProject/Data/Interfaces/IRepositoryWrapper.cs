namespace _2024FinalYearProject.Data.Interfaces
{
    public interface IRepositoryWrapper
    {
        ITransactionRepository Transaction { get; }
        IReviewRepository Review { get; }
        IChargesRepository Charges { get; }
        IBankAccountRepository BankAccount { get; }

        INotificationRepository Notification { get; }
    }
}
