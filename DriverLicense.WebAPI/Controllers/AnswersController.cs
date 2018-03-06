using System.Collections.Generic;
using DriverLicense.Interfaces.Maps;
using DriverLicense.Interfaces.Services;
using DriverLicense.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DriverLicense.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class AnswersController : Controller
    {
        private readonly IAnswersService _answersService;
        private readonly IAnswersMap _answersMap;

        public AnswersController(IAnswersService answersService
            , IAnswersMap answersMap)
        {
            _answersService = answersService;
            _answersMap = answersMap;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<AnswerDTO> Get()
        {
            return _answersMap.GetAll(_answersService.Fetch());
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public AnswerDTO Get(int id)
        {
            return _answersMap.Get(_answersService.Get(id));
        }
    }
}
