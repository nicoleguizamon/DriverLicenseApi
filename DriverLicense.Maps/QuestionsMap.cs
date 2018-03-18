using AutoMapper;
using DriverLicense.Interfaces.Maps;
using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using System.Collections.Generic;

namespace DriverLicense.Maps
{
    public class QuestionsMap : IQuestionsMap
    {
        private readonly IMapper _mapper;

        public QuestionsMap(IMapper mapper)
        {
            _mapper = mapper;
        }

        public QuestionDTO Get(Questions entity)
        {
            return _mapper.Map<Questions, QuestionDTO>(entity);
        }

        public Questions SetEntity(QuestionDTO entity)
        {
            return _mapper.Map<QuestionDTO, Questions>(entity);
        }

        public IEnumerable<QuestionDTO> GetAll(IEnumerable<Questions> entities)
        {
            return _mapper.Map<IEnumerable<Questions>, IEnumerable<QuestionDTO>>(entities);
        }
    }
}
