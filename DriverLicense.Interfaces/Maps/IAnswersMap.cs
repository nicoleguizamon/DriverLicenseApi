using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using System.Collections.Generic;

namespace DriverLicense.Interfaces.Maps
{
    public interface IAnswersMap
    {
        IEnumerable<AnswerDTO> GetAll(IEnumerable<Answers> entities);
        AnswerDTO Get(Answers entity);
        Answers SetEntity(AnswerDTO entity);
    }
}
