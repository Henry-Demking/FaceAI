
using Azure.Storage;
using Azure.Storage.Blobs;
using FaceAI.Classes;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FaceAI.Azure.Database
{
    class BlobHandler
    {
        static readonly string BLOB_KEY = "PdpA+IDe5XkRQ/1HYx8CtaPtbMUa+JkydAbrJbv8eKosVuouW6YFARct+QzyhpobHaCjhFzA8RtCA+fyi8tJfw==";
        static readonly string CONTAINER = "faces";

        static readonly string CONNECTION = ConfigurationManager.AppSettings.Get("BLOB_ENDPOINT");
        public static async Task<BlobImage> UploadToStorage(string path, string fileName)
        {
            // Location of the blob and the file to be stored on that blob
            string url = "https://6221faces.blob.core.windows.net/faces/" + fileName;
            Uri blobUri = new Uri(url);

            // Create credentials
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential("6221faces", BLOB_KEY);

            // Create client
            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            // Create File Stream
            FileStream fileStream = File.OpenRead(path);

            // Upload file to blob
            await blobClient.UploadAsync(fileStream);

            fileStream.Close();

            BlobImage image = new BlobImage(fileName, url);
            return image;
        }

        public static async Task<bool> DownloadToTemp(string path, string fileName)
        {
            string storageAccount_connectionString = "DefaultEndpointsProtocol=https;AccountName=6221faces;AccountKey=PdpA+IDe5XkRQ/1HYx8CtaPtbMUa+JkydAbrJbv8eKosVuouW6YFARct+QzyhpobHaCjhFzA8RtCA+fyi8tJfw==;EndpointSuffix=core.windows.net";

            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(storageAccount_connectionString);
            CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER);
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fileName);

            // provide the file download location below            
            Stream file = File.OpenWrite(path);    
          

            await cloudBlockBlob.DownloadToStreamAsync(file);

            file.Close();
            return true;
        }

        public static async void DeleteItem(string fileName)
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(CONNECTION);
            CloudBlobClient blobClient = storage.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER);

            var blob = container.GetBlockBlobReference(fileName);
            await blob.DeleteIfExistsAsync();
        }

        public static async Task<List<string>> GetFilesAsync()
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(CONNECTION);
            CloudBlobClient blobClient = storage.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER);
            BlobContinuationToken blobContinuationToken = null;

            var list = await container.ListBlobsSegmentedAsync(
                prefix              : null,
                useFlatBlobListing  : true,
                blobListingDetails  : BlobListingDetails.None,
                maxResults          : null,
                currentToken        : blobContinuationToken,
                options             : null,
                operationContext    : null
                );

            blobContinuationToken = list.ContinuationToken;
            List<string> files = new List<string>();

            foreach (IListBlobItem blob in list.Results)
            {
                var blobFileName = blob.Uri.Segments.Last();
                files.Add(blobFileName);
            }

            return files;
        }
    }
}
