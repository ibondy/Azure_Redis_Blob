using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using StackExchange.Redis;

namespace Redis_Example
{
    public class BlobReader
    {
        private readonly CloudBlobClient _client;
        public BlobReader(string storageAccountName, string storageAccountKey)
        {
            var credentials = new StorageCredentials(storageAccountName,storageAccountKey);

            // Retrieve storage account from connection string.
            var storageAccount = new CloudStorageAccount(credentials,true);

            // Create the blob client.
             _client = storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// Read from blob file 
        /// </summary>
        /// <remarks>Expected file format is 1 record per line and key,value format </remarks>
        /// <param name="containerReference"></param>
        /// <param name="blobName"></param>
        /// <param name="redisConnectionString"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public async Task ReadBlob(string containerReference, string blobName, string redisConnectionString, int batchSize = 10000)
        {
            var memStream = new MemoryStream();
            var container = _client.GetContainerReference(containerReference);
            var blob = container.GetBlobReference(blobName);
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Downloading blob from storage");
            await blob.DownloadToStreamAsync(memStream);
            memStream.Position = 0;
            Console.WriteLine($"Blob downloaded in {sw.Elapsed.Seconds} seconds");
            
            await  WriteRedis(redisConnectionString, batchSize, memStream);
           
        }

        private async Task WriteRedis(string redisConnectionString, int batchSize, MemoryStream memStream)
        {
            Console.WriteLine($"Starting to write Redis at {DateTime.Now}");
            var sw = new Stopwatch();
            sw.Start();
            var totalCount = 0;
            using (var reader = new StreamReader(memStream))
            {
                var counter = 0;

                var batch = new Dictionary<RedisKey, RedisValue>();
                while (!reader.EndOfStream)
                {
                    // assuming data format of xxxx,yyyyy
                    var line = reader.ReadLine()?.Split(new char[] {','});
                    if (line != null)
                    {
                        batch.Add(line[0], line[1]);
                    }

                    counter++;
                    totalCount++;

                    if (counter == batchSize) // batch size
                    {
                        var loader = new RedisLoader(redisConnectionString);
                        var loaderBatch = new Dictionary<RedisKey, RedisValue>(batch);
                        await loader.LoadItems(batch);
                        // prepare new batch
                        counter = 0;
                        batch.Clear();
                    }
                }
            }
            Console.WriteLine($"Finished to write Redis at {DateTime.Now}");
            Console.WriteLine($"Total of {totalCount} items pushed into Azure Redis in {sw.Elapsed.TotalSeconds} seconds");
        }
    }
}
