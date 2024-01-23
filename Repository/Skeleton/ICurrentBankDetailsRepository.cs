using Spider_EMT.Models;

namespace Spider_EMT.Repository.Skeleton
{
    public interface ICurrentBankDetailsRepository
    {
        Task<CurrentBankDetails> GetCurrentBankDetails();
    }
}
