using System;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace MongoDB.Succinctly.FileManager
{
    public class MongoDbFileManager
    {
        public static void UploadFile()
        {
            var database = DatabaseHelper.GetDatabaseReference("localhost", "file_store");

            IGridFSBucket bucket = new GridFSBucket(database);

            byte[] source = File.ReadAllBytes("sample.pdf");

            var id = bucket.UploadFromBytes("sample.pdf", source);

            Console.WriteLine(id.ToString());
        }

        public static void UploadFileFromAStream()
        {
            var database = DatabaseHelper.GetDatabaseReference("localhost", "file_store");

            IGridFSBucket bucket = new GridFSBucket(database);
            Stream stream = File.Open("sample.pdf", FileMode.Open);


            var options = new GridFSUploadOptions()
            {
                Metadata = new BsonDocument()
                {
                    {"author", "Mark Twain"},
                    {"year", 1900}
                }
            };

            var id = bucket.UploadFromStream("sample1.pdf", stream, options);

            Console.WriteLine(id.ToString());
        }

        public static async Task DownloadFileAsBytes()
        {
            var database = DatabaseHelper.GetDatabaseReference("localhost", "file_store");

            IGridFSBucket bucket = new GridFSBucket(database);

            var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Filename, "sample2.pdf");

            var searchResult = await bucket.FindAsync(filter);
            var fileEntry = searchResult.FirstOrDefault();

            byte[] content = await bucket.DownloadAsBytesAsync(fileEntry.Id);

            File.WriteAllBytes("C:\\temp\\sample3.pdf", content);

            //System.Diagnostics.Process.Start("C:\\temp\\sample2.pdf");
        }

        public static async Task DownloadFileAsBytesByName()
        {
            var database = DatabaseHelper.GetDatabaseReference("localhost", "file_store");

            IGridFSBucket bucket = new GridFSBucket(database);

            byte[] content = await bucket.DownloadAsBytesByNameAsync("sample2.pdf");

            File.WriteAllBytes("C:\\temp\\sample2.pdf", content);

            //System.Diagnostics.Process.Start("C:\\temp\\sample2.pdf");
        }

        public static async Task DownloadFileToStream()
        {
            var database = DatabaseHelper.GetDatabaseReference("localhost", "file_store");

            IGridFSBucket bucket = new GridFSBucket(database);

            var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Filename, "sample2.pdf");

            var searchResult = await bucket.FindAsync(filter);
            var fileEntry = searchResult.FirstOrDefault();

            var fileName = "c:\\temp\\mystream.pdf";
            using (Stream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                await bucket.DownloadToStreamAsync(fileEntry.Id, fileStream);

                fileStream.Close();
            }
        }

        public static async Task DownloadFileToStreamByName()
        {
            var database = DatabaseHelper.GetDatabaseReference("localhost", "file_store");

            IGridFSBucket bucket = new GridFSBucket(database);

            var fileName = "c:\\temp\\mystream2.pdf";
            using (Stream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                await bucket.DownloadToStreamByNameAsync("sample2.pdf", fileStream);

                fileStream.Close();
            }
        }
    }
}