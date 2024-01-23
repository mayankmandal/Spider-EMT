using Spider_EMT.Models;

namespace Spider_EMT.Repository.Skeleton
{
    public interface ITransactionFeeRepository
    {
        Task<TransactionFee> GetTransactionFee();
        Task<bool> UpdateTransactionFees(TransactionFee transactionFee);
        Task<bool> AddTransactionFees(TransactionFee transactionFee);
    }
}
