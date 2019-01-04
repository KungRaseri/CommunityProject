using MongoDB.Bson.Serialization.Attributes;

namespace Data.Helpers
{
    public interface IMongoEntity
    {
        [BsonId] string Id { get; set; }
    }
}