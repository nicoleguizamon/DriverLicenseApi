using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DriverLicense.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> ObjectSet();
        IQueryable<T> FindAll();
        IQueryable<T> FindAllBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAllBy(string query);
        T FindBy(Expression<Func<T, bool>> predicate);
        T FindBy(string query);
        IEnumerable<T> Query(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Remove(T entity);
        IList<T> ExecuteStoredProcedureByName(string procedure, object[] parameters);
    }
}
