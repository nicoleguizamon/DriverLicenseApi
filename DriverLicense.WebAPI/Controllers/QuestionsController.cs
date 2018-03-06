using System;
using System.Collections.Generic;
using System.Linq;
using DriverLicense.Interfaces.Maps;
using DriverLicense.Interfaces.Services;
using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DriverLicense.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class QuestionsController : Controller
    {
        private readonly ILocationsService _locationsService;
        private readonly IQuestionsService _questionsService;
        private readonly IQuestionsMap _questionsMap;

        public QuestionsController(IQuestionsService questionsService
            , ILocationsService locationsService
            , IQuestionsMap questionsMap)
        {
            _questionsService = questionsService;
            _locationsService = locationsService;
            _questionsMap = questionsMap;
        }


        // GET: api/<controller>
        [HttpGet("Locations/{locationId}")]
        public IEnumerable<QuestionDTO> Get(int locationId)
        {
            var numQuestionsToAsk = _locationsService.Get(locationId).QuestionsForTest;

            var list = _questionsService.FetchWithInclude(new List<string> { "Answers" }
                , x => x.LocationId == locationId).Select(z => new Questions
                {
                    Description = z.Description,
                    Image = z.Image,
                    LocationId = z.LocationId,
                    QuestionId = z.QuestionId,
                    Answers = z.Answers.Select(q => new Answers
                    {
                        AnswerId = q.AnswerId,
                        Description = q.Description,
                        IsCorrect = q.IsCorrect,
                        QuestionId = q.QuestionId
                    }).ToList()
                }).Take(3);//.OrderBy(q => Guid.NewGuid()).Take(numQuestionsToAsk);


            return _questionsMap.GetAll(list);
        }

        // GET api/<controller>/5
        [HttpGet("{questionId}")]
        public QuestionDTO GetQuestion(int questionId)
        {
            return _questionsMap.Get(_questionsService.Get(questionId, new List<string> { "Answers" }));
        }

       
    }
}
