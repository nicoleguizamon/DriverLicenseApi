using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriverLicense.Interfaces.Maps;
using DriverLicense.Interfaces.Services;
using DriverLicense.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DriverLicense.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionsService _questionsService;
        private readonly IQuestionsMap _questionsMap;

        public QuestionsController(IQuestionsService questionsService, IQuestionsMap questionsMap)
        {
            _questionsService = questionsService;
            _questionsMap = questionsMap;
        }


        // GET: api/<controller>
        [HttpGet("Locations/{locationId}")]
        public IEnumerable<QuestionDTO> Get(int locationId)
        {
            return _questionsMap.GetAll(_questionsService.FetchWithInclude(new List<string> { "Answers"}
            ,x=>x.LocationId==locationId));
        }

        // GET api/<controller>/5
        [HttpGet("{questionId}")]
        public string GetQuestion(int questionId)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
