using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MongoDB.Succinctly.MovieManager.Model
{
    public class BsonMapper
    {
        public static void Map()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Movie)))
            {
                BsonClassMap.RegisterClassMap<Movie>(movie =>
                {
                    movie.MapIdProperty(p => p.MovieId).SetIdGenerator(new StringObjectIdGenerator());
                    movie.MapProperty(p => p.Name).SetElementName("name");
                    movie.MapProperty(p => p.Director).SetElementName("directorName");
                    movie.MapProperty(p => p.Year).SetElementName("year");
                    movie.MapProperty(p => p.Actors).SetElementName("actors");
                    movie.UnmapProperty(p => p.Age);
                    movie.MapExtraElementsMember(p => p.Metadata);
                    movie.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Actor)))
            {
                BsonClassMap.RegisterClassMap<Actor>(actor =>
                {
                    actor.MapProperty(p => p.Name).SetElementName("name");
                });
            }
        }
    }
}