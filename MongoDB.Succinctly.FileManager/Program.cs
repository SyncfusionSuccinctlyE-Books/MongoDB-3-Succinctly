using System;

namespace MongoDB.Succinctly.FileManager
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                /*Uploading files */
                MongoDbFileManager.UploadFile();
                MongoDbFileManager.UploadFileFromAStream();

                MongoDbFileManager.DownloadFileAsBytes().Wait();
                MongoDbFileManager.DownloadFileAsBytesByName().Wait();

                MongoDbFileManager.DownloadFileToStream().Wait();

                MongoDbFileManager.DownloadFileToStreamByName().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Read();
        }
    }
}
