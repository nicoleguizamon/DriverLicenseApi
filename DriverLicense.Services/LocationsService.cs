using DriverLicense.Interfaces.Repositories;
using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using Microsoft.Extensions.Logging;
using DriverLicense.Interfaces.Services;

namespace DriverLicense.Services
{
    public class LocationsService : BaseService<Locations, LocationDTO>, ILocationsService
    {
        public LocationsService(IRepository<Locations> repository, ILogger<Locations> logger
            , IUnitOfWork persist) : base(repository, logger, persist, "CityId")
        {
        }
    }
}
