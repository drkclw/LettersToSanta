using Azure;
using Azure.AI.TextAnalytics;
using CognitiveServicesLibrary.Models;

namespace CognitiveServicesLibrary
{
    public class TextAnalyticsService
    {
        private string _endpoint { get; set; }
        private string _key { get; set; }
        private TextAnalyticsClient? _client { get; set; }
        public TextAnalyticsService(string endpoint, string key)
        {
            _endpoint = endpoint;
            _key = key;
        }

        private void Authenticate()
        {
            if(_client == null)
            {
                _client = new TextAnalyticsClient(new Uri(_endpoint), new AzureKeyCredential(_key));
            }            
        }

        public async Task<IEnumerable<EntityResult>> RecognizeEntities(string document)
        {
            Authenticate();
            var response = await _client.RecognizeEntitiesAsync(document);

            return response.Value.Select(e => new EntityResult
            {
                Text = e.Text,
                Category = e.Category.ToString(),
                Score = e.ConfidenceScore
            });
        }

        public async Task<IEnumerable<SentimentResult>> AnalyzeSentiment(string document)
        {
            Authenticate();
            var response = await _client.AnalyzeSentimentAsync(document);

            return response.Value.Sentences.Select(s => new SentimentResult
            {
                Text = s.Text,
                TextSentiment = s.Sentiment                
            });
        }

        public async Task<string> DetectLanguage(string document)
        {
            Authenticate();
            var response = await _client.DetectLanguageAsync(document);

            return response.Value.Name;
        }
    }
}
