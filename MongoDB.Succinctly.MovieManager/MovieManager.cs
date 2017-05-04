using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Permissions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Succinctly.MovieManager.Model;

namespace MongoDB.Succinctly.MovieManager
{
    /// <summary>
    /// Examples showing how to manipulate data for the simple operations such as
    ///     Create (insert new items to the database)
    ///     Read (list items from the 
    ///     Update (Update existing items)
    ///     Delete (delete items)
    /// </summary>
    public class MovieManager
    {
        /// <summary>
        /// Returns a list of movies as an array of BsonDocument
        /// </summary>
        /// <returns></returns>
        public static BsonDocument[] GetBsonMovies()
        {
            BsonDocument sevenSamurai = new BsonDocument()
            {
                {"name", "The Seven Samurai"},
                {"directorName", " Akira Kurosawa"},
                {
                    "actors", new BsonArray
                    {
                        new BsonDocument("name", "Toshiro Mifune"),
                        new BsonDocument("name", "Takashi Shimura")
                    }
                },
                {"year", 1954}
            };

            BsonDocument theGodfather = new BsonDocument()
            {
                {"name", "The Godfather"},
                {"directorName", "Francis Ford Coppola"},
                {
                    "actors", new BsonArray
                    {
                        new BsonDocument("name", "Marlon Brando"),
                        new BsonDocument("name", "Al Pacino"),
                        new BsonDocument("name", "James Caan")
                    }
                },
                {"year", 1972}
            };

            return new BsonDocument[] { sevenSamurai, theGodfather };
        }

        /// <summary>
        /// Returns a list of movies as an array of Movie objects
        /// </summary>
        /// <returns></returns>
        public static ModelWithAttributes.Movie[] GetMoviesWithAttributes()
        {
            ModelWithAttributes.Movie sevenSamurai = new ModelWithAttributes.Movie()
            {
                Name = "Seven Samurai",
                Director = "Akira Kurosawa",
                Year = 1954,
                Actors = new[]
                {
                    new ModelWithAttributes.Actor {Name = "Toshiro Mifune"},
                    new ModelWithAttributes.Actor {Name = "Takashi Shimura"},
                }
            };

            ModelWithAttributes.Movie theGodFather = new ModelWithAttributes.Movie()
            {
                Name = "The Godfather",
                Director = "Francis Ford Coppola",
                Year = 1972,
                Actors = new[]
                {
                    new ModelWithAttributes.Actor {Name = "Marlon Brando"},
                    new ModelWithAttributes.Actor {Name = "Al Pacino"},
                },
                Metadata = new BsonDocument("href", "http://thegodfather.com")
            };
            return new[] { sevenSamurai, theGodFather };
        }

        /// <summary>
        /// Returns a list of movies as an array of Movie objects
        /// </summary>
        /// <returns></returns>
        public static Movie[] GetMovieList()
        {
            Movie sevenSamurai = new Movie()
            {
                Name = "The Seven Samurai",
                Director = "Akira Kurosawa",
                Year = 1954,
                Actors = new Actor[]
                {
                    new Actor {Name = "Toshiro Mifune"},
                    new Actor {Name = "Takashi Shimura"},
                }
            };

            Movie theGodFather = new Movie()
            {
                Name = "The Godfather",
                Director = "Francis Ford Coppola",
                Year = 1972,
                Actors = new Actor[]
                {
                    new Actor {Name = "Marlon Brando"},
                    new Actor {Name = "Al Pacino"},
                },
                Metadata = new BsonDocument("href", "http://thegodfather.com")
            };

            Movie cabaretMovie = new Movie()
            {
                Name = "Cabaret",
                Director = "	Bob Fosse",
                Year = 1972,
                Actors = new Actor[]
                {
                    new Actor {Name = "Liza Minnelli"},
                    new Actor {Name = "Michael York"},
                },
                Metadata = new BsonDocument("href", "https://en.wikipedia.org/wiki/Cabaret_(1972_film)")
            };


            return new Movie[] { sevenSamurai, theGodFather, cabaretMovie };
        }

        public static async Task Insert<T>(T[] movies, string dbName, string collName)
        {
            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);

            var moviesCollection = db.GetCollection<T>(collName);
            await moviesCollection.InsertManyAsync(movies);
        }
         
        public static async void FindMoviesAsDocuments(string dbName, string collName)
        {
            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<BsonDocument>(collName);
            var filter = new BsonDocument();
            int count = 0;
            using (var cursor = await collection.FindAsync<BsonDocument>(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        var movieName = document.GetElement("name").Value.ToString();
                        Console.WriteLine("Movie Name: {0}", movieName);
                        count++;
                    }
                }
            }
        }

        public static async void FindMoviesAsObjects(string dbName, string collName)
        {
            Console.WriteLine("************************* FindMoviesAsObjects *********************");
            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);
            var filter = new BsonDocument();
            int count = 0;
            using (var cursor = await collection.FindAsync<Movie>(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var movie in batch)
                    {

                        Console.WriteLine("Movie Name: {0}", movie.Name);

                        count++;
                    }
                }
            }
        }

        public static void FindMoviesByUsingLinq(string dbName, string collName)
        {
            Console.WriteLine("************************* FindMoviesByUsingLinq *********************");
            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            var movies = collection.AsQueryable()
                .Where(x => x.Name == "The Godfather")
                .Select(x => new { MovieName = x.Name, MainActor = x.Actors[0].Name });

            foreach (var movie in movies)
            {
                Console.WriteLine("Movie name: {0} and Main Actor: {1}", movie.MovieName, movie.MainActor);
            }
        }

        public static async void FindMoviesByName(string dbName, string collName, string movieName)
        {
            Console.WriteLine("************************* FindMoviesByName *********************");
            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            /* 1. Filter to retrieve movies where the name equals to "The Godfather" */
            var filter = Builders<Movie>.Filter.Eq(x => x.Name, movieName);

            /* 2. Filter to retrieve movies where the name equals to "The Godfather" by using BsonDocument notation 
            var filter = Builders<BsonDocument>.Filter.Eq("name", "The Godfather");
            */

            /* 3. find movies where the name is "The Godfather" OR "The Seven Samurai" 
            var filter = Builders<Movie>.Filter.Or(new[]
            {
                new ExpressionFilterDefinition<Movie>(x => x.Name == "The Godfather"),
                new ExpressionFilterDefinition<Movie>(x => x.Name == "The Seven Samurai")
            });
            */

            /* 4. piping queries 
            var query = filter.Eq("name", movieName) | filter.Gt("year", 1900);
            */

            var sort = Builders<Movie>.Sort.Ascending(x => x.Name).Descending(x => x.Year);

            var result = await collection
                .Find(filter)
                .Sort(sort)
                .ToListAsync();

            foreach (var movie in result)
            {
                Console.WriteLine("Match found: movie with name '{0}' exists", movie.Name);
            }
        }

        public static void FindMoviesByNameWithProjection(string dbName, string collName, string movieName)
        {
            Console.WriteLine("************************* Projecting Movies *********************");
            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            var projection = Builders<Movie>.Projection
                .Include("name")
                .Include("year")
                .Exclude("_id");

            var data = collection.Find(new BsonDocument())
                .Project<BsonDocument>(projection)
                .ToList();

            foreach (var item in data)
            {
                Console.WriteLine("Item retrieved {0}", item.ToString());
            }
        }

        public static async void FindMoviesWithProjectionsAsync(string dbName, string collName, string movieName)
        {
            Console.WriteLine("************************* Projecting Movies *********************");
            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            var projection = Builders<Movie>.Projection
                .Include("name")
                .Include("year")
                .Exclude("_id");

            var options = new FindOptions<Movie, BsonDocument>
            {
                Projection = projection
            };
            var cursor = await collection.FindAsync(new BsonDocument(), options);
            var data = cursor.ToList();

            foreach (var item in data)
            {
                Console.WriteLine("Item retrieved {0}", item.ToString());
            }
        }

        public static void AggregateMovies(string dbName, string collName)
        {
            Console.WriteLine("************************* Aggregating Movies *********************");

            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            var data = collection.Aggregate()
                .Group(new BsonDocument
                {
                    {"_id", "$year"},
                    {"count", new BsonDocument("$sum", 1)}
                });

            foreach (var item in data.ToList())
            {
                Console.WriteLine("Item retrieved {0}", item.ToString());
            }
        }

        public static void AggregateMoviesWithFiltering(string dbName, string collName)
        {
            Console.WriteLine("************************* Aggregating Movies With Filtering *********************");

            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);


            var aggregate = collection.Aggregate()
                .Match(Builders<Movie>.Filter.Where(x => x.Name.Contains("Samurai")))
                .Group(new BsonDocument
                {
                    {"_id", "$year"},
                    {"count", new BsonDocument("$sum", 1)}
                });

            var results = aggregate.ToList();

            foreach (var item in results)
            {
                Console.WriteLine("Item retrieved {0}", item.ToString());
            }
        }

        public static void UpdateMovie(string dbName, string collName)
        {
            Console.WriteLine("************************* UpdateMovie *********************");

            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            var builder = Builders<Movie>.Filter;
            var filter = builder.Eq("name", "The Godfathesr");
            var update = Builders<Movie>.Update
                .Set("name", "new name")
                .Set("newProperty", "something goes here")
                .Set(d => d.Year, 1900);


            var updateOptions = new FindOneAndUpdateOptions<Movie, Movie>()
            {
                ReturnDocument = ReturnDocument.After,
                Projection = Builders<Movie>
                    .Projection
                    .Include(x => x.Year)
                    .Include(x => x.Name),
            };

            //-- method 1 FindOneAndUpdate
            Movie result = collection.FindOneAndUpdate(filter, update, updateOptions);

            //-- method 2 Update One
            //UpdateResult result = collection.UpdateOne(filter, update);

            //-- method 3 UpdateMany
            //UpdateResult result = collection.UpdateMany(filter, update);

            Console.WriteLine(result.ToBsonDocument());
        }

        public static void UpdateOrInsertMovie(string dbName, string collName)
        {
            Console.WriteLine("************************* UpdateMovie *********************");

            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            var builder = Builders<Movie>.Filter;
            var filter = builder.Eq("name", "The Godfathesr");
            var update = Builders<Movie>.Update
                .Set("name", "new name")
                .Set("newProperty", "something goes here")
                .Set(d => d.Year, 1900)
                .SetOnInsert("_id", StringObjectIdGenerator.Instance.GenerateId(null, null));

            var updateOptions = new FindOneAndUpdateOptions<Movie, Movie>()
            {
                ReturnDocument = ReturnDocument.After,
                Projection = Builders<Movie>
                    .Projection
                    .Include(x => x.Year)
                    .Include(x => x.Name),
                IsUpsert = true
            };

            Movie result = collection.FindOneAndUpdate(filter, update, updateOptions);

            Console.WriteLine(result.ToBsonDocument());
        }

        public static async void Replace(string dbName, string collName)
        {
            Console.WriteLine("************************* ReplaceOne *********************");

            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            var builder = Builders<Movie>.Filter;
            var filter = builder.Eq("name", "The Godfather");

            //find the ID of the godfather movie...
            var theGodfather = await collection.FindAsync(filter);
            var theGodfatherMovie = theGodfather.FirstOrDefault();

            Movie replacementMovie = new Movie
            {
                MovieId = theGodfatherMovie.MovieId,
                Name = "Mad Max: Fury Road",
                Year = 2015,
                Actors = new[]
                {
                    new Actor {Name = "Tom Hardy"},
                    new Actor {Name = "Charlize Theron"},
                },
                Director = "George Miller"
            };


            ReplaceOneResult result = await collection.ReplaceOneAsync(filter, replacementMovie);

            Console.WriteLine(result.ToBsonDocument());
        }

        public static async void Delete(string dbName, string collName)
        {
            Console.WriteLine("************************* Delete *********************");

            var db = DatabaseHelper.GetDatabaseReference("localhost", dbName);
            var collection = db.GetCollection<Movie>(collName);

            var builder = Builders<Movie>.Filter;
            var filter = builder.Eq("name", "The Godfather");

            //DeleteResult result = await collection.DeleteOneAsync(m => m.Name == "The Seven Samurai");
            //var result = await collection.DeleteManyAsync(m => m.Name == "The Seven Samurai" || m.Name == "Cabaret");

            //var builder = Builders<Movie>.Filter;
            //var filter = builder.Eq("name", "The Godfather");
            //var result = await collection.DeleteManyAsync(filter);

            BsonDocument result =
                await
                    collection.FindOneAndDeleteAsync(m => m.Name == "Cabaret",
                        new FindOneAndDeleteOptions<Movie, BsonDocument>
                        {
                            Sort = Builders<Movie>.Sort.Ascending(x => x.Name),
                            Projection = Builders<Movie>.Projection.Include(x => x.MovieId)
                        });

            Console.WriteLine(result.ToBsonDocument());
        }
    }
}