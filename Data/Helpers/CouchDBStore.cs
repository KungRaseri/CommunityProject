using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Interfaces;
using Data.Models;
using Data.Models.Twitch;
using MyCouch;
using MyCouch.Requests;
using MyCouch.Responses;
using Newtonsoft.Json;

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
        public virtual async Task<ViewQueryResponse<T>> CreateView(string viewName, string suffix = "all", string emitParamOne = "doc._id", string emitParamTwo = "doc")
        {
            var view = await Client.Views.QueryAsync<T>(new QueryViewRequest(EntityName, $"{viewName}-{suffix}"));

            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ViewQueryResponse> CreateGetView(string map)
        {
            var viewRequest = new QueryViewRequest(EntityName, $"{EntityName}-all");

            var view = await Client.Views.QueryAsync(viewRequest);
            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Row<T>>> GetAsync(string viewFilter = null, string key = null)
        {
            var query = new Query(EntityName, (string.IsNullOrEmpty(viewFilter)) ? $"{EntityName}-all" : viewFilter) { Key = key };

            var resultTest = await Store.QueryAsync<T>(query);

            return resultTest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewFilter"></param>
        /// <returns></returns>
        public virtual async Task<T> FindAsync(string id, string viewFilter = null)
        {
            var query = new Query(EntityName, (string.IsNullOrEmpty(viewFilter)) ? $"{EntityName}-all" : viewFilter) { Key = id };

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

            if (typeof(T) == typeof(Vod))
            {
                var vod = entity as Vod;

                var query = new Query(EntityName, $"{EntityName}-by-video-id") { Key = vod?.Video.Id };
                var existingRows = await Store.QueryAsync<T>(query);

                var record = existingRows.FirstOrDefault()?.Value as Vod;

                if (record != null)
                {
                    record.Video = vod?.Video;
                    response = await Client.Documents.PutAsync(record._id, Client.Serializer.Serialize(record));
                }
                else
                {
                    response = await Client.Documents.PostAsync(Client.Serializer.Serialize(entity));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(entity._id))
                {
                    response = await Client.Documents.PutAsync(entity._id, entity._rev, Client.Serializer.Serialize(entity));
                }
                else
                {
                    response = await Client.Documents.PostAsync(Client.Serializer.Serialize(entity));
                }
            }

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

            if (!string.IsNullOrEmpty(entity._id))
            {
                response = await Client.Documents.PutAsync(entity._id, Client.Serializer.Serialize(entity));
            }
            else
            {
                response = await Client.Documents.PostAsync(Client.Serializer.Serialize(entity));
            }

            var result = await Store.GetByIdAsync<T>(response.Id);

            return result;
        }
    }
}
