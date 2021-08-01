using log4net;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WoA.Lib.Persistence.MongoDB
{
    public class MongoWrapper : IGenericRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly ILog _logger;

        private IMongoCollection<T> GetCollection<T>()
        {
            while (_database == null)
                Thread.Sleep(100);
            return _database.GetCollection<T>(typeof(T).Name);
        }

        public MongoWrapper(IStylizedConsole console, ILog logger, IConfiguration config)
        {
            try
            {
                _logger = logger;
                string uri = $"mongodb+srv://{config.DbUser}:{config.DbPassword}@cluster0.gzsuu.mongodb.net/WoA?retryWrites=true&w=majority&connect=replicaSet";
                uri = "mongodb://localhost";
                _client = new MongoClient(uri);
                _database = _client.GetDatabase("WoA");
            }
            catch (Exception e)
            {
                _logger.Error("Error while trying to create Mongo Client or getting database:", e);
            }
        }

        public T Add<T>(T model)
        {
            try
            {
                IMongoCollection<T> collection = GetCollection<T>();
                collection.InsertOne(model);
                return model;
            }
            catch (Exception e)
            {
                _logger.Error("Error in MongoWrapper.Add", e);
                return model;
            }
        }

        public void AddAll<T>(List<T> models)
        {
            try
            {
                if (models.Any())
                {
                    IMongoCollection<T> collection = GetCollection<T>();
                    collection.InsertMany(models);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error in MongoWrapper.AddAll", e);
            }
        }

        public bool Delete<T>(T model) where T : IIdentifiable
        {
            try
            {
                IMongoCollection<T> collection = GetCollection<T>();
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", model.Id);
                DeleteResult result = collection.DeleteOne(filter);
                return result.DeletedCount == 1;
            }
            catch (Exception e)
            {
                _logger.Error("Error in MongoWrapper.Delete", e);
                return false;
            }
        }

        public void DeleteAll<T>(List<T> models) where T : IIdentifiable
        {
            try
            {
                if (models.Any())
                {
                    IMongoCollection<T> collection = GetCollection<T>();
                    FilterDefinition<T> filter = Builders<T>.Filter.AnyIn("Id", models.Select(m => m.Id));
                    DeleteResult result = collection.DeleteMany(filter);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error in MongoWrapper.DeleteAll", e);
            }
        }

        public T[] GetAll<T>()
        {
            try
            {
                IMongoCollection<T> collection = GetCollection<T>();
                IFindFluent<T, T> findFluent = collection.Find(f => true);
                List<T> ts = findFluent.ToList();
                return ts.ToArray();
            }
            catch (Exception e)
            {
                _logger.Error("Error in MongoWrapper.GetAll", e);
                return new T[] { };
            }
        }

        public T GetById<T>(string id) where T : IIdentifiable, new()
        {
            try
            {
                IMongoCollection<T> collection = GetCollection<T>();
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
                return collection.Find(filter).FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.Error("Error in MongoWrapper.GetById", e);
                throw;
            }
        }

        public T Update<T>(T model) where T : IIdentifiable
        {
            try
            {
                IMongoCollection<T> collection = GetCollection<T>();
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", model.Id);
                collection.ReplaceOne(filter, model);
                return model;
            }
            catch (Exception e)
            {
                _logger.Error("Error in MongoWrapper.Update", e);
                throw;
            }
        }

        public void UpdateAll<T>(List<T> models) where T : IIdentifiable
        {
            try
            {
                models.ForEach(m => Update(m));
            }
            catch (Exception e)
            {
                _logger.Error("Error in MongoWrapper.UpdateAll", e);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
