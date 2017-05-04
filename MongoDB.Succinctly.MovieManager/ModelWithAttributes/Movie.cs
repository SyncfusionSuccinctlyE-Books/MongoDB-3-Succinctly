using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MongoDB.Succinctly.MovieManager.ModelWithAttributes
{
    [BsonIgnoreExtraElements]
    public class Movie
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string MovieId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("directorName")]
        public string Director { get; set; }

        [BsonElement("actors")]
        public Actor[] Actors { get; set; }

        [BsonElement("year")]
        [BsonRepresentation(BsonType.Double)]
        public int Year { get; set; }

        [BsonIgnore]
        public int Age
        {
            get { return DateTime.Now.Year - this.Year; }
        }

        [BsonExtraElements]
        public BsonDocument Metadata { get; set; }
    }
}
