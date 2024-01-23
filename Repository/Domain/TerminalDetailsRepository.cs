using AutoMapper;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Data;

namespace Spider_EMT.Repository.Domain
{
    public class TerminalDetailsRepository : ITerminalDetailsRepository
    {
        private readonly ISSDataRepository _sSDataRepository;
        private readonly IMapper _mapper;
        public TerminalDetailsRepository(ISSDataRepository sSDataRepository, IMapper mapper)
        {
            _sSDataRepository = sSDataRepository;
            _mapper = mapper;
        }
        public async Task<TerminalDetails> GetTerminalDetails(string terminalId)
        {
            var AllSsData = _sSDataRepository.GetSsData();
            var termSSData = AllSsData.Result.FirstOrDefault(ss => ss.TermId == terminalId);
            var terminalDetailsData = _mapper.Map<TerminalDetails>(termSSData);
            return terminalDetailsData;
        }
    }
}
