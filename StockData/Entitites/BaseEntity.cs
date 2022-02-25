using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace StockData.Entitites
{
    public class BaseEntity<TKey>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual TKey Id { get; set; }
        public virtual DateTime InsertedDate { get; set; }

    }
}
