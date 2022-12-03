using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using CognitiveServicesLibrary;
using DatabaseLibrary.Models;
using DatabaseLibrary;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace TextExtractionFunction
{
    public class TextExtractor
    {
        private readonly IConfiguration _configuration;

        public TextExtractor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("TextExtraction")]
        public void Run([BlobTrigger("letterimages/{name}", Source = BlobTriggerSource.EventGrid, Connection = "StorageCS")]Stream myBlob, string name, ILogger log)
        {
            List<string> lines = ExtractText(myBlob).Result;
            var language = DetectLanguage(lines).Result;
            var needsAttention = NeedsAttention(lines).Result;
            var requestedItems = GetItems(lines).Result;

            var santaLetter = new SantaLetter
            {
                Id = name + DateTime.Now.Year.ToString(),
                PartitionKey = DateTime.Now.Year.ToString(),
                FileName = name,
                IsLetter = lines.Count > 0,
                IsRead = false,
                Content = lines.ToArray(),
                Language = language,
                NeedsAttention = needsAttention,
                Requesteditems = requestedItems
            };

            SaveLetter(santaLetter);

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }

        private async Task<List<string>> ExtractText(Stream imageStream)
        {
            VisionService visionClient = new VisionService(_configuration["CognitiveServicesUrl"], 
                _configuration["CognitiveServicesKey"]);

            return await visionClient.ExtractText(imageStream);
        }

        private async void SaveLetter(SantaLetter letter)
        {
            DatabaseClient dbClient = new DatabaseClient(_configuration["CosmosEndpointUri"], _configuration["CosmosPrimaryKey"],
                _configuration["CosmosLetterContainerId"], _configuration["CosmosLetterDatabaseId"],
                _configuration["FunctionApplicationName"]);

            await dbClient.AddItemToContainerAsync(letter);
        }

        private async Task<string> DetectLanguage(List<string> lines)
        {
            TextAnalyticsService textAnalyticsService = new TextAnalyticsService(_configuration["CognitiveServicesUrl"],
                _configuration["CognitiveServicesKey"]);

            StringBuilder sb = new StringBuilder();
            foreach(string line in lines)
            {
                sb.AppendLine(line);
            }

            return await textAnalyticsService.DetectLanguage(sb.ToString());
        }
        
        private async Task<bool> NeedsAttention(List<string> lines)
        {
            TextAnalyticsService textAnalyticsService = new TextAnalyticsService(_configuration["CognitiveServicesUrl"],
                _configuration["CognitiveServicesKey"]);

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            var sentimentResults = await textAnalyticsService.AnalyzeSentiment(sb.ToString());

            return sentimentResults.Any(sr => sr.TextSentiment == Azure.AI.TextAnalytics.TextSentiment.Negative);
        }

        private async Task<string[]> GetItems(List<string> lines)
        {
            TextAnalyticsService textAnalyticsService = new TextAnalyticsService(_configuration["CognitiveServicesUrl"],
                _configuration["CognitiveServicesKey"]);

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            var entityRecognitionResults = await textAnalyticsService.RecognizeEntities(sb.ToString());

            return entityRecognitionResults.Where(er => er.Category == "Product" && er.Score > 0.7)
                .Select(ir => ir.Text).ToArray();
        }
    }
}
