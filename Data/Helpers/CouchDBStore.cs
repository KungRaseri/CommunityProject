using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Interfaces;
using Data.Models;
using MyCouch;
using MyCouch.Requests;
using MyCouch.Responses;

namespace Data.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CouchDbStore<T> where T : class, IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        private MyCouchClient Client { get; }

        /// <summary>
        /// 
        /// </summary>
        private static string EntityName => typeof(T).Name.ToLower();

        /// <summary>
        /// 
        /// </summary>
        private MyCouchServerClient CouchServerClient { get; }

        /// <summary>
        /// 
        /// </summary>
        public MyCouchStore Store { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CouchDbStore(string couchDbUrl)
        {
            if (string.IsNullOrEmpty(couchDbUrl))
            {
                throw new ArgumentException($"couchDbUrl parameter must not be null or empty. :mehdiNERD:");
            }

            var dbConnectionInfo = new DbConnectionInfo(couchDbUrl, EntityName);

            Client = new MyCouchClient(dbConnectionInfo);

            // If the database doesn't exist, create it.
            CouchServerClient = new MyCouchServerClient(couchDbUrl);

            Store = new MyCouchStore(Client);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<GetDatabaseResponse> GetDatabase()
        {
            return await CouchServerClient.Databases.GetAsync(EntityName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<DatabaseHeaderResponse> CreateDatabase()
        {
            var request = new PutDatabaseRequest(EntityName);
            return await CouchServerClient.Databases.PutAsync(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<DatabaseHeaderResponse> DeleteDatabase()
        {
            var request = new DeleteDatabaseRequest(EntityName);
            return await CouchServerClient.Databases.DeleteAsync(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ViewQueryResponse> CreateGetView(string map)
        {
            var viewDocument = new
            {
                views = new { view = new { map = map } }
            };

            var viewRequest = new QueryViewRequest(EntityName, $"{EntityName}-all");

            var view = await Client.Views.QueryAsync(viewRequest);
            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Row<T>>> GetAsync()
        {
            var query = new Query(EntityName, $"{EntityName}-all");

            var resultTest = await Store.QueryAsync<T>(query);

            return resultTest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(string id)
        {
            var query = new Query(EntityName, $"{EntityName}-all") { Key = id };

            var result = await Store.QueryAsync<T>(query);

            return result.FirstOrDefault()?.Value;
        }

        /// <summary>
        /// An entity without the _id field passed in will add (POST) instead of update (PUT)
        /// </summary>
        /// <param name="entity">The entity to add or update</param>
        /// <returns></returns>
        public virtual async Task<T> AddOrUpdateAsync(T entity)
        {
            DocumentHeaderResponse response;

            if (!string.IsNullOrEmpty(entity.Id) )
            {
                response = await Client.Documents.PutAsync(entity.Id, Client.Serializer.Serialize(entity));
            }
            else
            {
                response = await Client.Documents.PostAsync(Client.Serializer.Serialize(entity));
            }

            //response = await Store.StoreAsync(entity.Id, Client.Serializer.Serialize(entity));

            var result = await Store.GetByIdAsync<T>(response.Id);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(string id)
        {
            return await Store.DeleteAsync(id);
        }

        /// <summary>
        /// An entity without the _id field passed in will add (POST) instead of update (PUT)
        /// </summary>
        /// <param name="entity">The entity to add or update</param>
        /// <returns></returns>
        public virtual async Task<T> AddOrUpdateTokenAsync(T entity)
        {
            if (typeof(T) != typeof(Token))
                return null;

            DocumentHeaderResponse response;

            if (!string.IsNullOrEmpty(entity.Id))
            {
                response = await Client.Documents.PutAsync(entity.Id, Client.Serializer.Serialize(entity));
            }
            else
            {
                response = await Client.Documents.PostAsync(Client.Serializer.Serialize(entity));
            }

            //response = await Store.StoreAsync(entity.Id, Client.Serializer.Serialize(entity));

            var result = await Store.GetByIdAsync<T>(response.Id);

            return result;
        }

        public virtual async Task<T> FindUserByEmail(string email)
        {
            if (typeof(T) != typeof(User))
                return null;

            var query = new Query(EntityName, $"{EntityName}-email") { Key = email };

            var response = await Store.QueryAsync<T>(query);

            return response.FirstOrDefault()?.Value;
        }

        public virtual async Task<T> FindTokenByUserId(string userId)
        {
            if (typeof(T) != typeof(Token))
                return null;

            var query = new Query(EntityName, $"{EntityName}-userId") { Key = userId };

            var response = await Store.QueryAsync<T>(query);

            return response.FirstOrDefault()?.Value;
        }
    }
}
