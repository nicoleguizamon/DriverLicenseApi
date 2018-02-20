using DriverLicense.Interfaces.Repositories;
using DriverLicense.Interfaces.Services;
using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using Microsoft.Extensions.Logging;

namespace DriverLicense.Services
{
    public class QuestionsService : BaseService<Questions, QuestionDTO>, IQuestionsService
    {
        public QuestionsService(IRepository<Questions> repository, ILogger<Questions> logger
            , IUnitOfWork persist) : base(repository, logger, persist, "QuestionId")
        {
        }
    }
}
