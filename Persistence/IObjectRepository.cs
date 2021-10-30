using System;
using System.Linq;
using System.Linq.Expressions;

namespace FFXIVVenues.Api.Persistence
{
    public interface IObjectRepository
    {
        void Upsert<T>(T entity) where T : class, IEntity;
        void Delete<T>(T entity) where T : class, IEntity;
        void Delete<T>(string id) where T : class, IEntity;
        bool Exists<T>(string id) where T : class, IEntity;
        IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity;
        IQueryable<T> GetAll<T>() where T : class, IEntity;
        T GetById<T>(string id) where T : class, IEntity;
    }

}
