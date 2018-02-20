using DriverLicense.Interfaces.Repositories;
using DriverLicense.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace DriverLicense.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DriverLicenseContext context;
        private DbSet<T> objectSet;
        string errorMessage = string.Empty;

        public Repository(DriverLicenseContext context)
        {
            this.context = context;
            objectSet = context.Set<T>();
        }
        public DbSet<T> ObjectSet()
        {
            return objectSet;
        }

        public IQueryable<T> FindAll()
        {
            return objectSet;
        }

        public IQueryable<T> FindAllBy(System.Linq.Expressions.Expression<Func<T, bool>> Predicate)
        {
            return objectSet.Where(Predicate);
        }

        public IQueryable<T> FindAllBy(string query)
        {
            return objectSet.Where(query);
        }

        public IEnumerable<T> Query(System.Linq.Expressions.Expression<Func<T, bool>> Predicate)
        {
            return FindAllBy(Predicate);
        }

        public T FindBy(System.Linq.Expressions.Expression<Func<T, bool>> Predicate)
        {
            return FindAllBy(Predicate).FirstOrDefault();
        }

        public T FindBy(string query)
        {
            var resultQuery = objectSet.Where(query);
            var result = resultQuery.SingleOrDefault();
            return result;
        }

        public void Add(T Entity)
        {
            objectSet.Add(Entity);
        }

        public void Delete(T Entity)
        {
            objectSet.Remove(Entity);
        }

        void IRepository<T>.Remove(T Entity)
        {
            Delete(Entity);
        }
        public IList<T> ExecuteStoredProcedureByName(string procedure, object[] parameters)
        {
            return objectSet.FromSql<T>(procedure, parameters).ToList<T>();
        }
    }
}
