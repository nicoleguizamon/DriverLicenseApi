using DriverLicense.Interfaces.Repositories;
using DriverLicense.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DriverLicense.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DriverLicenseContext _context;

        public UnitOfWork(DriverLicenseContext context)
        {
            _context = context;
        }

        public void Commit()
        {

            var validationErrors = _context.ChangeTracker.Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(r => r != ValidationResult.Success);

            if (validationErrors.Any())
            {

                //  throw an exception here
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Determine if the entity is attached to Entity Framework DbContext.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool ExistsInContext<T>(T entity) where T : class
        {
            return _context.Set<T>().Local.Any(e => e == entity);
        }

        /// <summary>
        /// Disconnect an entity from Entity Framework DbContext.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
	    public void DisconnectEntity<T>(T entity) where T : class
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// Disconnect a list of a given entity from Entity Framework DbContext.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        public void DisconnectEntity<T>(List<T> entityList) where T : class
        {
            foreach (var indEntity in entityList)
            {
                _context.Entry(indEntity).State = EntityState.Detached;
            }
        }
    }
}
