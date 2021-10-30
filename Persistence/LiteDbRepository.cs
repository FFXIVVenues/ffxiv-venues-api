using LiteDB;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace FFXIVVenues.Api.Persistence
{

    // This repository should remain singleton. While it will work fine none-singleton, LiteDb opens,
    // locks, unlocks and closes the target database file as the LiteDatabase object is instantiated
    // and disposed, all of which are very expensive. Page caching is also kept at LiteDatabase level,
    // so is lost on disposing it. Additionally, LiteDb is thread-safe. So, it's considerably more 
    // performant to keep a continual instance of LiteDatabase. 
    // For more information, see here: https://github.com/mbdavid/LiteDB/wiki/Concurrency
    public class LiteDbRepository : IObjectRepository, IDisposable
    {

        private readonly LiteDatabase _repository;

        public LiteDbRepository(string strConnectionString)
        {
            var connectionString = new ConnectionString(strConnectionString);
            if (!Path.IsPathRooted(connectionString.Filename))
            {
                var assembliesPath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().Location!).LocalPath);
                connectionString.Filename = Path.Combine(assembliesPath, connectionString.Filename);
            }
            Directory.CreateDirectory(Path.GetDirectoryName(connectionString.Filename));
            _repository = new LiteDatabase(connectionString);
        }

        public void Upsert<T>(T entity) where T : class, IEntity =>
            _repository.GetCollection<T>().Upsert(entity);

        public void Delete<T>(T entity) where T : class, IEntity =>
            _repository.GetCollection<T>().Delete(entity.Id);

        public void Delete<T>(string id) where T : class, IEntity =>
            _repository.GetCollection<T>().Delete(id);

        public IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity =>
            _repository.GetCollection<T>().Find(predicate).AsQueryable();

        public IQueryable<T> GetAll<T>() where T : class, IEntity =>
            _repository.GetCollection<T>().FindAll().AsQueryable();

        public T GetById<T>(string id) where T : class, IEntity =>
            _repository.GetCollection<T>().FindById(id);

        public bool Exists<T>(string id) where T : class, IEntity =>
            _repository.GetCollection<T>().FindById(id) != null;

        public void Dispose() =>
            Dispose(true);

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~LiteDbRepository() =>
            Dispose(false);

    }
}
