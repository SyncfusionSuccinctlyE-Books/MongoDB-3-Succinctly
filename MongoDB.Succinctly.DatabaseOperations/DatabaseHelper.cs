using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Succinctly.DatabaseOperations.Examples
{
    public class DatabaseHelper
    {
        private static string hostName = "localhost";
        public static async Task GetListOfDatabasesAsync()
        {
            MongoClient client = GetMongoClient("localhost");

            Console.WriteLine("Getting the list of databases asynchronously…");
            using (var cursor = await client.ListDatabasesAsync())
            {
                await cursor.ForEachAsync(d => Console.WriteLine(d.ToString()));
            }
        }

        public static void GetListOfDatabasesSync()
        {
            MongoClient client = GetMongoClient(hostName);

            Console.WriteLine("Getting the list of databases synchronously…");
            var databases = client.ListDatabases().ToList();
            databases.ForEach(d => Console.WriteLine(d.GetElement("name").Value));
        }

        public static IMongoDatabase CreateDatabase(string databaseName, string collectionName)
        {
            MongoClient client = GetMongoClient(hostName);
            IMongoDatabase database = client.GetDatabase(databaseName);

            if (!DoesCollectionExists(database, collectionName))
            {
                //checking if the collection exists as otherwise an exception is returned!
                database.CreateCollection(collectionName);
            }
            return database;
        }

        private static bool DoesCollectionExists(IMongoDatabase database, string collectionName)
        {

            var filter = new BsonDocument("name", collectionName);
            //filter by collection name
            var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });
            //check for existence
            return collections.ToList().Any();
        }
        public static void DropDatabase(string databaseName)
        {
            MongoClient client = GetMongoClient(hostName);
            client.DropDatabase(databaseName);
        }

        static async void DropDatabaseAsync(string databaseName)
        {
            MongoClient client = GetMongoClient(hostName);
            await client.DropDatabaseAsync(databaseName);
        }

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

        public static void ConnectWithoutAuthentication()
        {
            string connectionString = "mongodb://localhost:27017";

            MongoClient client = new MongoClient(connectionString);

            Console.WriteLine("Connected");
        }

        public static void ConnectWithAuthentication()
        {
            string dbName = "ecommlight";
            string userName = "some_user";
            string password = "pwd";

            var credentials = MongoCredential.CreateCredential(dbName, userName, password);

            MongoClientSettings clientSettings = new MongoClientSettings()
            {
                Credentials = new[] { credentials },
                Server = new MongoServerAddress(hostName, 27017)
            };

            MongoClient client = new MongoClient(clientSettings);

            Console.WriteLine("Connected as {0}", userName);
        }



        public static void CreateListAndDropCollections(string databaseName)
        {
            var database = GetDatabaseReference(hostName, databaseName);
            var collectionName = "some_collection";

            //create a new collection
            database.CreateCollection(collectionName);

            //showing the list of collection before deleting
            ListCollections(hostName, databaseName);

            //delete a collection
            database.DropCollection(collectionName);

            //showing the list of collections after deleting
            ListCollections(hostName, databaseName);
        }

        public static void ListCollections(string hostName, string databaseName)
        {
            var database = GetDatabaseReference(hostName, databaseName);

            var collectionsList = database.ListCollections();

            Console.WriteLine("List of collections in the {0} database:", database.DatabaseNamespace);
            foreach (var collection in collectionsList.ToList())
            {
                Console.WriteLine(collection.ToString());
            }
        }
    }
}
