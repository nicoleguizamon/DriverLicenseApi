using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DriverLicense.Interfaces.Services
{
    public interface IService<T, V>
        where T : class
        where V : class
    {
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate = null);
        IQueryable<T> Fetch(string query);
        T Get(int uniqueID);
        T Get(int uniqueID, List<string> includeSubset);
        bool SimpleSave(T programModel, string userID, bool commit = false);
        bool SimpleUpdate(V programViewModel, string userID, bool commit = false, List<string> updateExclusions = null);
        T UpdateAndReturn(V programViewModel, string userID, bool commit = false, List<string> updateExclusions = null);
        T SaveAndReturn(T programModel, string userID, bool commit = false);
        void Add(T entity);
        bool SaveHelper(T programModel, string userID, bool commit = false);
        bool UpdateHelper(T programModel, string userID, bool commit = false);
        IQueryable<T> FetchWithInclude(List<string> includeSubset, Expression<Func<T, bool>> predicate = null);
        bool Delete(int uniqueID, string userID, bool commit = false);
        bool LogicalDelete(int uniqueID, string userID, bool commit = false);
    }
}
