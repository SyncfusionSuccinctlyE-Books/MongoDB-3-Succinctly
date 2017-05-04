using System;
using MongoDB.Succinctly.DatabaseOperations.Examples;

namespace MongoDB.Succinctly.DatabaseOperations
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /* Operations on database */
            DatabaseHelper.ConnectWithoutAuthentication();
            DatabaseHelper.ConnectWithAuthentication();

            DatabaseHelper.CreateDatabase("newDbName", "exampleCollection");

            DatabaseHelper.GetListOfDatabasesAsync().Wait();
            DatabaseHelper.GetListOfDatabasesSync();

            /**************************************
             * Operations on collections 
             ***************************************/
            var newDatabaseName = "newDbName";
            DatabaseHelper.CreateListAndDropCollections(newDatabaseName);

            Console.Read();
        }
    }
}