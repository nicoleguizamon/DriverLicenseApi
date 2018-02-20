using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using System.Collections.Generic;

namespace DriverLicense.Interfaces.Maps
{
    public interface IQuestionsMap
    {
        IEnumerable<QuestionDTO> GetAll(IEnumerable<Questions> entities);
        QuestionDTO Get(Questions entity);
        Questions SetEntity(QuestionDTO entity);
    }
}
