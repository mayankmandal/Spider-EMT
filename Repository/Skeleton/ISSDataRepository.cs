using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface ISSDataRepository
    {
        Task<IEnumerable<SSDataViewModel>> GetSsData();
    }
}
