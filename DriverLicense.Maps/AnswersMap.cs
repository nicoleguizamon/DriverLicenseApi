using AutoMapper;
using DriverLicense.Interfaces.Maps;
using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using System.Collections.Generic;

namespace DriverLicense.Maps
{
    public class AnswersMap : IAnswersMap
    {
        private readonly IMapper _mapper;

        public AnswersMap(IMapper mapper)
        {
            _mapper = mapper;
        }

        public AnswerDTO Get(Answers entity)
        {
            return _mapper.Map<Answers, AnswerDTO>(entity);
        }

        public Answers SetEntity(AnswerDTO entity)
        {
            return _mapper.Map<AnswerDTO, Answers>(entity);
        }

        public IEnumerable<AnswerDTO> GetAll(IEnumerable<Answers> entities)
        {
            return _mapper.Map<IEnumerable<Answers>, IEnumerable<AnswerDTO>>(entities);
        }
    }
}
