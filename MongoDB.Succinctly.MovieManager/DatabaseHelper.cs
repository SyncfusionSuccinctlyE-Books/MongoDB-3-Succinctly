using MongoDB.Driver;

namespace MongoDB.Succinctly.MovieManager
{
    public class DatabaseHelper
    {
        private static readonly string hostName = "localhost";

        public static MongoClient GetMongoClient(string hostName)
        {
            string connectionString = string.Format("mongodb://{0}:27017", hostName);
            return new MongoClient(connectionString);
        }

        public static IMongoDatabase GetDatabaseReference(string hostName, string dbName)
        {
            MongoClient client = GetMongoClient(hostName);
            IMongoDatabase database = client.GetDatabase(dbName);
            return database;
        }

        public static void DropDatabase(string databaseName)
        {
            MongoClient client = GetMongoClient(hostName);
            client.DropDatabase(databaseName);
        }
    }
}