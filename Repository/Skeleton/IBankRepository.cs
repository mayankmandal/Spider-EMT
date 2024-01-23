using Spider_EMT.Models;

namespace Spider_EMT.Repository.Skeleton
{
    public interface IBankRepository
    {
        Task<IEnumerable<BankReferenceData>> GetBanks();
    }
}
