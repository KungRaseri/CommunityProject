using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Interfaces;
using Data.Models;
using Data.Models.Twitch;
using MongoDB.Bson;
using MongoDB.Driver;
using MyCouch;
using MyCouch.Responses;
using Newtonsoft.Json;

namespace Data.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MongoDbStore<T> where T : class, IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        private IMongoClient Client { get; }

        private IMongoDatabase Database { get; }

        /// <summary>
        /// 
        /// </summary>
        private static string EntityName => typeof(T).Name.ToLower();

        /// <summary>
        /// 
        /// </summary>
        public MongoDbStore(string dbUrl)
        {
            if (string.IsNullOrEmpty(dbUrl))
            {
                throw new ArgumentException($"dbUrl parameter must not be null or empty.");
            }

            Client = new MongoClient(dbUrl);
            Database = GetDatabase();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IMongoDatabase GetDatabase()
        {
            return Database ?? Client.GetDatabase(EntityName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task CreateDatabase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<object> DeleteDatabase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAsync(string viewFilter = null, string key = null)
        {
            return await Database.GetCollection<T>(EntityName).Find(new BsonDocument()).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewFilter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAsync(string id)
        {
            var collection = Database.GetCollection<T>(EntityName);
            var filter = new FilterDefinitionBuilder<T>()
                            .Where(t => t._id == id);
            return await collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> Save(T entity)
        {
            var collection = Database.GetCollection<T>(EntityName);

            await collection.ReplaceOneAsync(x => x._id.Equals(entity._id), entity, new UpdateOptions
            {
                IsUpsert = true
            });

            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(string id)
        {
            var collection = Database.GetCollection<T>(EntityName);

            await collection.DeleteOneAsync(x => x._id.Equals(id));
        }
    }
}
