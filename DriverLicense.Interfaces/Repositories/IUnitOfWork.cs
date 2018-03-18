using System.Collections.Generic;

namespace DriverLicense.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        void Commit();
        bool ExistsInContext<T>(T entity) where T : class;
        void DisconnectEntity<T>(T entity) where T : class;
        void DisconnectEntity<T>(List<T> entityList) where T : class;
    }
}
