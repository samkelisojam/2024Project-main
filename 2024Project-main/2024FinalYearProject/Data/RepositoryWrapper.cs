using _2024FinalYearProject.Data.Interfaces;

namespace _2024FinalYearProject.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly AppDbContext _appDbContext;
        private IChargesRepository _chargesRepository;
        private ITransactionRepository _Transaction;
        private IReviewRepository _Review;
        private INotificationRepository _Notification;
        private IBankAccountRepository _bankAccount;
        private ILoginRepository _logins;
        public RepositoryWrapper(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IBankAccountRepository BankAccount
        {
            get
            {
                if (_bankAccount == null)
                {
                    _bankAccount = new BankAccountRepository(_appDbContext);
                }

                return _bankAccount;
            }
        }

        public ILoginRepository Logins
        {
            get
            {
                if (_logins == null)
                {
                    _logins = new LoginRepository(_appDbContext);
                }

                return _logins;
            }
        }

        public ITransactionRepository Transaction
        {
            get
            {
                if (_Transaction == null)
                {
                    _Transaction = new TransactionRepository(_appDbContext);
                }

                return _Transaction;
            }
        }

        public INotificationRepository Notification
        {
            get
            {
                if (_Notification == null)
                {
                    _Notification = new NotificationRepository(_appDbContext);
                }

                return _Notification;
            }
        }

        public IChargesRepository Charges
        {
            get
            {
                if (_chargesRepository == null)
                {
                    _chargesRepository = new ChargesRepository(_appDbContext);
                }

                return _chargesRepository;
            }
        }

        public IReviewRepository Review
        {
            get
            {
                if (_Review == null)
                {
                    _Review = new ReviewRepository(_appDbContext);
                }

                return _Review;
            }
        }
        public void SaveChanges()
        {
            _appDbContext.SaveChanges();
        }
    }
}
