using AutoMapper;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<SSDataViewModel, TerminalDetails>().ReverseMap();
        }
    }
}
