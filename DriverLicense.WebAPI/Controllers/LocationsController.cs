using System.Collections.Generic;
using DriverLicense.Interfaces.Maps;
using DriverLicense.Interfaces.Services;
using DriverLicense.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DriverLicense.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class LocationsController : Controller
    {
        private readonly ILocationsService _locationsService;
        private readonly ILocationsMap _locationsMap;

        public LocationsController(ILocationsService locationsService
            , ILocationsMap locationsMap
            , IQuestionsMap questionsMap)
        {
            _locationsService = locationsService;
            _locationsMap = locationsMap;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<LocationDTO> Get()
        {
            return _locationsMap.GetAll(_locationsService.Fetch());
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public LocationDTO Get(int id)
        {
            return _locationsMap.Get(_locationsService.Get(id));
        }
        
    }
}
