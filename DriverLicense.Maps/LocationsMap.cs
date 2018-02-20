using AutoMapper;
using DriverLicense.Interfaces.Maps;
using DriverLicense.Models.DTOs;
using DriverLicense.Models.Models;
using System.Collections.Generic;

namespace DriverLicense.Maps
{
    public class LocationsMap : ILocationsMap
    {
        private readonly IMapper _mapper;

        public LocationsMap(IMapper mapper)
        {
            _mapper = mapper;
        }

        public LocationDTO Get(Locations entity)
        {
            return _mapper.Map<Locations, LocationDTO>(entity);
        }

        public Locations SetEntity(LocationDTO entity)
        {
            return _mapper.Map<LocationDTO, Locations>(entity);
        }

        public IEnumerable<LocationDTO> GetAll(IEnumerable<Locations> entities)
        {
            return _mapper.Map<IEnumerable<Locations>, IEnumerable<LocationDTO>>(entities);
        }
    }
}
