using MongoDB.Bson.Serialization;

namespace MongoDB.Succinctly.MovieManager
{
    public class MovieIdGenerator : IIdGenerator
    {
        public object GenerateId(object container, object document)
        {

            return "Movie_" + System.Guid.NewGuid().ToString();
        }

        public bool IsEmpty(object id)
        {
            return id == null || string.IsNullOrEmpty(id.ToString());
        }
    }
}