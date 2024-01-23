using Spider_EMT.Models;

namespace Spider_EMT.Repository.Skeleton
{
    public interface IAtmTransactionRepository
    {
        Task<IEnumerable<AtmTransactionData>> GetAllTransactions(DateTime StartDate, DateTime EndDate);
    }
}
