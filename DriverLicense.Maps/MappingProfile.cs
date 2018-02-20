using AutoMapper;
using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;

namespace DriverLicense.Maps
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Answers, AnswerDTO>();
            CreateMap<Locations, LocationDTO>();
            CreateMap<Questions, QuestionDTO>();
        }
    }
}
