using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace TextExtractionFunction
{
    public class TextExtraction
    {
        private readonly IConfiguration _configuration;

        public TextExtraction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("TextExtraction")]
        public void Run([BlobTrigger("letterimages/{name}", Source = BlobTriggerSource.EventGrid, Connection = "StorageCS")]Stream myBlob, string name, ILogger log)
        {
            var test = _configuration["StorageUrl"];
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
