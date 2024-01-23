using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface ITerminalDetailsRepository
    {
        Task<TerminalDetails> GetTerminalDetails(string terminalId);
    }
}
