using DriverLicense.ErrorHandling;
using DriverLicense.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Linq.Dynamic.Core;

namespace DriverLicense.Services
{
    public abstract class BaseService<T, V>
        where T : class
        where V : class
    {
        protected readonly IRepository<T> _repository;
        protected readonly string _repoUniqueIDColumn;
        protected readonly IUnitOfWork _persist;
        protected readonly List<string> _updateExclusions;

        protected const string CreateDateColumn = "CreatedAt";
        protected const string CreateUserIDColumn = "CreatedBy";
        protected const string UpdateDateColumn = "LastUpdatedAt";
        protected const string DeletedColumn = "Deleted";
        protected const string UpdateUserIDColumn = "LastUpdatedBy";
        protected readonly ILogger _logger;

        protected BaseService(IRepository<T> repository
            , ILogger<T> logger
            
            , IUnitOfWork persist
            , string repoUniqueIDColumn
            , List<string> updateExclusions = null)
        {
            _repository = repository;
            _persist = persist;
            _repoUniqueIDColumn = repoUniqueIDColumn;
            _updateExclusions = updateExclusions ?? new List<string>() { CreateDateColumn, CreateUserIDColumn };
            _logger = logger;
            
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate = null)
        {
            var repoQueryable = _repository.FindAll();
            if (predicate != null)
                repoQueryable = repoQueryable.Where(predicate);
            return repoQueryable;
        }

        public virtual IQueryable<T> FetchWithInclude(List<string> includeSubset, Expression<Func<T, bool>> predicate = null)
        {
            var repoQueryable = _repository.FindAll();
            if (predicate != null)
            {
                repoQueryable = repoQueryable.Where(predicate);
            }
            foreach (var subset in includeSubset)
            {
                repoQueryable = repoQueryable.Include(subset);
            }
            return repoQueryable;
        }

        public virtual IQueryable<T> Fetch(string query)
        {
            return _repository.FindAllBy(query);
        }

        public virtual T Get(int uniqueID)
        {
            var whereClause = _repoUniqueIDColumn + " = " + uniqueID.ToString();
            var response = _repository.FindBy(whereClause);
            return response;
        }

        public virtual void Add(T entity)
        {
            _repository.Add(entity);
        }

        public virtual T Get(string uniqueID)
        {
            var whereClause = _repoUniqueIDColumn + " = " + uniqueID;
            var response = _repository.FindBy(whereClause);
            return response;
        }

        public virtual T Get(int uniqueID, List<string> includeSubset)
        {
            var repoQueryable = _repository.FindAll();

            repoQueryable = repoQueryable.Where(_repoUniqueIDColumn + " = " + uniqueID.ToString());
            foreach (var subset in includeSubset)
            {
                repoQueryable = repoQueryable.Include(subset);
            }
            return repoQueryable.FirstOrDefault();
        }

        public virtual bool SimpleSave(T programModel, string userID, bool commit = false)
        {
            return Save(programModel, userID, commit) != null;
        }

        public virtual bool SimpleUpdate(T programModel, string userID, bool commit = false)
        {
            return Update(programModel, userID, commit) != null;
        }

        public virtual T UpdateAndReturn(V programViewModel, string userID, bool commit = false, List<string> updateExclusions = null)
        {
            return Update(programViewModel, userID, commit, updateExclusions);
        }

        public virtual T SaveAndReturn(T programModel, string userID, bool commit = false)
        {
            return Save(programModel, userID, commit);
        }

        public virtual bool SimpleUpdate(V programViewModel, string userID, bool commit = false, List<string> updateExclusions = null)
        {
            return Update(programViewModel, userID, commit, updateExclusions) != null;
        }

        public virtual T Update(V programModel, string userID, bool commit = false, List<string> updateExclusions = null)
        {
            int? programModelID = null;
            var modelType = typeof(T);
            T databaseModel;
            try
            {
                List<string> exclusions = updateExclusions == null ? _updateExclusions : updateExclusions.Union(_updateExclusions).ToList();
                programModelID = GetUniqueID(programModel);

                if (programModelID != null && programModelID.Value != 0)
                {
                    databaseModel = Get(programModelID.Value);
                    if (databaseModel == null)
                    {
                        throw new HttpException(HttpStatusCode.NotFound, string.Format("ERROR UPDATING {0}, ID ({1}) NOT FOUND", modelType.Name, programModelID));
                    }
                    databaseModel = OverlayViewModel(programModel, databaseModel, exclusions);

                    var updateDateProperty = modelType.GetProperty(UpdateDateColumn);
                    updateDateProperty?.SetValue(databaseModel, DateTimeOffset.UtcNow, null);

                    var updateUserIDProperty = modelType.GetProperty(UpdateUserIDColumn);
                    updateUserIDProperty?.SetValue(databaseModel, userID, null);
                }
                else
                {
                    _logger.LogError(LoggingEvents.UPDATE_ITEM, "ERROR UPDATING {0} WITH ID ({1})", modelType.Name, programModelID);
                    throw new HttpException(HttpStatusCode.NotFound, string.Format("ERROR UPDATING {0}, ID ({1}) NOT FOUND", modelType.Name, programModelID));
                }
                if (commit)
                {
                    _persist.Commit();
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("column "))
                {
                    throw new HttpException(HttpStatusCode.BadRequest,
                        string.Format("ERROR UPDATING {0} with ID ({1}). Some values provided in the properties could not be correct.", modelType.Name, programModelID));
                }
                var error = new { username = userID, error = "Base Service Failed.", info = string.Format("ERROR UPDATING {0} with ID ({1}).", modelType.Name, programModelID) };
                
                throw new HttpException(HttpStatusCode.BadRequest, string.Format("ERROR UPDATING {0} with ID ({1}).", modelType.Name, programModelID));
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.UPDATE_ITEM, "ERROR UPDATING {0} WITH ID ({1}). Exception: {2}. InnerException: {3}"
                    , modelType.Name, programModelID, ex.Message, ex.InnerException != null ? ex.InnerException.Message : null);
                
                throw;
            }
            return databaseModel;
        }

        public virtual T Update(T programModel, string userID, bool commit = false, List<string> updateExclusions = null)
        {
            int? programModelID = null;
            var modelType = typeof(T);
            T databaseModel;
            try
            {
                List<string> exclusions = updateExclusions == null ? _updateExclusions : updateExclusions.Union(_updateExclusions).ToList();
                programModelID = GetUniqueID(programModel);

                if (programModelID != null && programModelID.Value != 0)
                {
                    databaseModel = Get(programModelID.Value);
                    if (databaseModel == null)
                    {
                        throw new HttpException(HttpStatusCode.NotFound, string.Format("ERROR UPDATING {0}, ID ({1}) NOT FOUND", modelType.Name, programModelID));
                    }
                    databaseModel = OverlayViewModel(programModel, databaseModel, exclusions);

                    var updateDateProperty = modelType.GetProperty(UpdateDateColumn);
                    updateDateProperty?.SetValue(databaseModel, DateTimeOffset.UtcNow, null);

                    var updateUserIDProperty = modelType.GetProperty(UpdateUserIDColumn);
                    updateUserIDProperty?.SetValue(databaseModel, userID, null);
                }
                else
                {
                    _logger.LogWarning(LoggingEvents.UPDATE_ITEM, "ERROR UPDATING {0}, ID ({1}) NOT FOUND. Tech: BaseService.Update.", modelType.Name, programModelID);
                    throw new HttpException(HttpStatusCode.NotFound, string.Format("ERROR UPDATING {0}, ID ({1}) NOT FOUND", modelType.Name, programModelID));
                }
                if (commit)
                {
                    _persist.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.UPDATE_ITEM, "ERROR UPDATING {0} WITH ID ({1}). Exception: {2}. InnerException: {3}"
                    , modelType.Name, programModelID, ex.Message, ex.InnerException != null ? ex.InnerException.Message : null);
               
                throw;
            }
            return databaseModel;
        }

        public virtual T Save(T programModel, string userID, bool commit = false)
        {
            T databaseModel = null;
            int? programModelID = null;
            var modelType = typeof(T);
            try
            {
                programModelID = GetUniqueID(programModel);

                if (programModelID != null && programModelID.Value <= 0)
                {
                    var createDateProperty = modelType.GetProperty(CreateDateColumn);
                    createDateProperty?.SetValue(programModel, DateTimeOffset.UtcNow, null);

                    var createUserIDProperty = modelType.GetProperty(CreateUserIDColumn);
                    createUserIDProperty?.SetValue(programModel, userID, null);

                    var updateDateProperty = modelType.GetProperty(UpdateDateColumn);
                    updateDateProperty?.SetValue(programModel, DateTimeOffset.UtcNow, null);

                    var updateUserIDProperty = modelType.GetProperty(UpdateUserIDColumn);
                    updateUserIDProperty?.SetValue(programModel, userID, null);

                    _repository.Add(programModel);
                    databaseModel = programModel;
                }
                else
                {
                    _logger.LogWarning(LoggingEvents.INSERT_ITEM, "ERROR SAVING {0}", modelType.Name);
                    throw new HttpException(HttpStatusCode.BadRequest, string.Format("ERROR SAVING {0}", modelType.Name));
                }
                if (commit)
                {
                    _persist.Commit();
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("column "))
                {
                    throw new HttpException(HttpStatusCode.BadRequest,
                        string.Format("ERROR SAVING {0} with ID ({1}). Some values provided in the properties could not be correct.", modelType.Name, programModelID));
                }
                
                throw new HttpException(HttpStatusCode.BadRequest, string.Format("ERROR SAVING {0} with ID ({1}).", modelType.Name, programModelID));
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.INSERT_ITEM, "ERROR SAVING {0}. Exception: {1}. InnerException: {2}", modelType.Name, ex.Message, ex.InnerException != null ? ex.InnerException.Message : null);
               
                throw new HttpException(HttpStatusCode.BadRequest, string.Format("ERROR SAVING {0}", modelType.Name));
            }
            return databaseModel;
        }

        public virtual bool UpdateHelper(T programModel, string userID, bool commit = false)
        {
            int? programModelID = null;
            var modelType = typeof(T);
            T databaseModel;
            try
            {
                programModelID = GetUniqueID(programModel);

                if (programModelID != null && programModelID.Value != 0)
                {
                    databaseModel = Get(programModelID.Value);
                    if (databaseModel == null)
                    {
                        throw new HttpException(HttpStatusCode.NotFound, string.Format("ERROR UPDATING {0}, ID ({1}) NOT FOUND", modelType.Name, programModelID));
                    }
                    databaseModel = OverlayViewModel(programModel, databaseModel, null);
                }
                else
                {
                    _logger.LogWarning(LoggingEvents.UPDATE_ITEM, "ERROR UPDATING {0}, ID ({1}) NOT FOUND. Tech: BaseService.UpdateHelper.", modelType.Name, programModelID);
                    throw new HttpException(HttpStatusCode.NotFound, string.Format("ERROR UPDATING {0}, ID ({1}) NOT FOUND", modelType.Name, programModelID));
                }
                if (commit)
                {
                    _persist.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.UPDATE_ITEM, "ERROR UPDATING {0} WITH ID ({1}). Exception: {2}. InnerException: {3}"
                    , modelType.Name, programModelID, ex.Message, ex.InnerException != null ? ex.InnerException.Message : null);
               
                throw new HttpException(HttpStatusCode.BadRequest, string.Format("ERROR UPDATING {0}", modelType.Name));
            }
            return databaseModel != null;
        }

        public virtual bool SaveHelper(T programModel, string userID, bool commit = false)
        {
            T databaseModel = null;
            var modelType = typeof(T);
            try
            {
                _repository.Add(programModel);
                databaseModel = programModel;
                if (commit)
                {
                    _persist.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.INSERT_ITEM, "ERROR SAVING {0}. Exception: {1}. InnerException: {2}", modelType.Name, ex.Message, ex.InnerException != null ? ex.InnerException.Message : null);
               
                throw new HttpException(HttpStatusCode.BadRequest, string.Format("ERROR SAVING {0}", modelType.Name));
            }
            return databaseModel != null;
        }

        public virtual bool Delete(int uniqueID, string userID, bool commit = false)
        {
            var programModel = Get(uniqueID);
            var modelType = typeof(T);
            if (programModel != null)
            {
                _repository.Delete(programModel);

                if (commit)
                {
                    _persist.Commit();
                }
            }
            else
            {
                _logger.LogWarning(LoggingEvents.DELETE_ITEM, "ERROR DELETING {0}, ID ({1}) NOT FOUND). Tech: BaseService.Delete", modelType.Name, uniqueID);
                throw new HttpException(HttpStatusCode.NotFound, string.Format("ERROR DELETING {0}, ID ({1}) NOT FOUND", modelType.Name, uniqueID));
            }

            return true;
        }

        public virtual bool LogicalDelete(int uniqueID, string userID, bool commit = false)
        {
            var programModel = Get(uniqueID);
            var modelType = typeof(T);
            if (programModel != null)
            {
                var deleteProperty = modelType.GetProperty(DeletedColumn);
                deleteProperty?.SetValue(programModel, true, null);

                var updateDateProperty = modelType.GetProperty(UpdateDateColumn);
                updateDateProperty?.SetValue(programModel, DateTimeOffset.UtcNow, null);

                var updateUserIDProperty = modelType.GetProperty(UpdateUserIDColumn);
                updateUserIDProperty?.SetValue(programModel, userID, null);

                if (commit)
                {
                    _persist.Commit();
                }
            }
            else
            {
                _logger.LogWarning(LoggingEvents.DELETE_ITEM, "ERROR DELETING {0}, ID ({1}) NOT FOUND. Tech: BaseService.LogicalDelete", modelType.Name, uniqueID);
                throw new HttpException(HttpStatusCode.NotFound, string.Format("ERROR DELETING {0}, ID ({1}) NOT FOUND", modelType.Name, uniqueID));
            }

            return true;
        }

        public static T OverlayViewModel(object viewModel, object databaseObject, List<string> excludeFields = null)
        {
            var viewType = viewModel.GetType();
            var dbType = databaseObject.GetType();

            var allVModelProperties = viewType.GetProperties();
            foreach (var indProp in allVModelProperties)
            {
                if (excludeFields == null || excludeFields.FindAll(x => x.IndexOf(indProp.Name, StringComparison.OrdinalIgnoreCase) >= 0).Count == 0)
                {
                    var dbProperty = dbType.GetProperty(indProp.Name);
                    if (dbProperty != null && (!dbProperty.PropertyType.GetTypeInfo().IsClass || dbProperty.PropertyType == typeof(string)))
                    {
                        var dbFieldType = dbProperty.PropertyType;
                        var viewFieldType = indProp.PropertyType;

                        if (dbFieldType.GetTypeInfo().IsGenericType && dbFieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            dbFieldType = Nullable.GetUnderlyingType(dbFieldType);
                        }

                        if (viewFieldType.GetTypeInfo().IsGenericType && viewFieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            viewFieldType = Nullable.GetUnderlyingType(viewFieldType);
                        }

                        if (dbFieldType == viewFieldType)
                        {
                            dbProperty.SetValue(databaseObject, indProp.GetValue(viewModel, null), null);
                        }
                    }
                }
            }
            return (T)databaseObject;
        }

        private int? GetUniqueID(object model)
        {
            int? uniqueID;
            var modelType = model.GetType();
            var uniqueIDProperty = modelType.GetProperty(_repoUniqueIDColumn);
            if (uniqueIDProperty.PropertyType == typeof(int))
            {
                uniqueID = (int)uniqueIDProperty.GetValue(model, null);
            }
            else
            {
                uniqueID = null;
            }

            return uniqueID;
        }
    }
}
