using DriverLicense.Interfaces.Repositories;
using DriverLicense.Interfaces.Services;
using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using Microsoft.Extensions.Logging;

namespace DriverLicense.Services
{
    public class AnswersService : BaseService<Answers, AnswerDTO>, IAnswersService
    {
        public AnswersService(IRepository<Answers> repository, ILogger<Answers> logger
            , IUnitOfWork persist) : base(repository, logger, persist, "AnswerId")
        {
        }
    }
}
