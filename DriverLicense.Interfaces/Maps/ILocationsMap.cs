using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using System.Collections.Generic;

namespace DriverLicense.Interfaces.Maps
{
    public interface ILocationsMap
    {
        IEnumerable<LocationDTO> GetAll(IEnumerable<Locations> entities);
        LocationDTO Get(Locations entity);
        Locations SetEntity(LocationDTO entity);
    }
}
