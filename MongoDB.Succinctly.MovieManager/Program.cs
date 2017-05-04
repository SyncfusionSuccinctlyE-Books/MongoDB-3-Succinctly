using System;
using MongoDB.Bson;
using MongoDB.Succinctly.MovieManager.Model;

namespace MongoDB.Succinctly.MovieManager
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string DB_NAME = "moviesDb";
            const string MOVIES_POCO_DB = "movies_poco";

            DatabaseHelper.DropDatabase(DB_NAME);

            /* Working with BSON documents */
            BsonDocument[] movies = MovieManager.GetBsonMovies();
            MovieManager.Insert<BsonDocument>(movies, DB_NAME, "movies_bson").Wait();

            /* Working with object model with attributes */
            ModelWithAttributes.Movie[] attrMovies = MovieManager.GetMoviesWithAttributes();
            MovieManager.Insert(attrMovies, DB_NAME, "movies_attr").Wait();

            /* Working with object model without attributes */
            Movie[] pocoMovies = MovieManager.GetMovieList();
            BsonMapper.Map(); //map the class to the MongoDB representation
            MovieManager.Insert<Movie>(pocoMovies, DB_NAME, MOVIES_POCO_DB).Wait();

            /*searching for all movies in the movies_bson table */
            
            MovieManager.FindMoviesAsDocuments(DB_NAME, "movies_bson");
            MovieManager.FindMoviesAsObjects(DB_NAME, MOVIES_POCO_DB);        
            Console.WriteLine("searching for a movie called 'The Seven Samurai'");
            MovieManager.FindMoviesByName(DB_NAME, MOVIES_POCO_DB, "The Seven Samurai");
            MovieManager.FindMoviesByNameWithProjection(DB_NAME, MOVIES_POCO_DB, "The Seven Samurai");
            MovieManager.FindMoviesWithProjectionsAsync(DB_NAME, MOVIES_POCO_DB, "The Seven Samurai");
            MovieManager.FindMoviesByUsingLinq(DB_NAME, MOVIES_POCO_DB);
            MovieManager.AggregateMovies(DB_NAME, MOVIES_POCO_DB);
            MovieManager.AggregateMoviesWithFiltering(DB_NAME, MOVIES_POCO_DB);
            
            /* updates */
            MovieManager.UpdateMovie(DB_NAME, MOVIES_POCO_DB);
            MovieManager.FindMoviesAsObjects(DB_NAME, MOVIES_POCO_DB);

            MovieManager.Replace(DB_NAME, MOVIES_POCO_DB);

            /* delete */
            MovieManager.Delete(DB_NAME, MOVIES_POCO_DB);
            
            Console.Read();
        }
    }
}