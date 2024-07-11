using Spider_EMT.Models;

namespace Spider_EMT.Repository.Skeleton
{
    public interface IErrorLogRepository
    {
        Task<int> LogErrorAsync(ErrorLog errorLog);
    }
}
