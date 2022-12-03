using Azure.AI.TextAnalytics;

namespace CognitiveServicesLibrary.Models
{
    public class SentimentResult
    {
        public TextSentiment TextSentiment { get; set; }
        public string? Text { get; set; }
    }
}
