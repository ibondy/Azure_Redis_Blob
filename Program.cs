using System;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace Redis_Example
{ 
    class Program
    {
        public static Stopwatch StopWatch { get; } = new Stopwatch();
        static void Main(string[] args)
        {
             
            //Console.WriteLine("Creating test file");
            //var testFile = new CreateTestFile();
            //testFile.CreateFile(10000000).GetAwaiter().GetResult();
            Console.WriteLine($"Starting job at {DateTime.Now}");
            var settings = ConfigurationManager.AppSettings;
            var job = new BlobReader(settings["StorageAccountName"],settings["BlobKey"] ); // update app.config file with your values
            job.ReadBlob(settings["BlobContainer"], settings["BlobName"], settings["RedisConnection"], 100000).GetAwaiter().GetResult();

            Console.WriteLine($"Job finished at {DateTime.Now} . Click any key to exit");
            Console.ReadLine();
      
        }
    }
}
