using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace CognitiveServicesLibrary
{
    public class VisionService
    {
        private string _endPoint { get; set; }
        private string _key { get; set; }
        public VisionService(string endPoint, string key)
        {
            _endPoint = endPoint;
            _key = key;
        }
        private ComputerVisionClient Authenticate()
        {
            var visionEndpoint = _endPoint;
            var visionKey = _key;

            var client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(visionKey))
              { Endpoint = visionEndpoint };
            return client;
        }

        public async Task<List<string>> ExtractText(Stream imageStream)
        {
            var client = Authenticate();
            // Read text from URL
            var textHeaders = await client.ReadInStreamAsync(imageStream);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            //Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            var lines = new List<string>();
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    //Console.WriteLine(line.Text);
                    lines.Add(line.Text);
                }
            }
            //Console.WriteLine();

            return lines;
        }
    }
}