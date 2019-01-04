using MongoDB.Bson.Serialization.Attributes;

namespace Data.Interfaces
{
    public interface IEntity
    {
        [BsonId]
        string _id { get; set; }
        string _rev { get; set; }
    }
}
