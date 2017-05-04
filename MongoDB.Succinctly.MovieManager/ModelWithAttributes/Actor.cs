using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Succinctly.MovieManager.ModelWithAttributes
{
    public class Actor
    {
        [BsonElement("name")]
        public string Name { get; set; }
    }
}