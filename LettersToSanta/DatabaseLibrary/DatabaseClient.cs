using DatabaseLibrary.Models;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using Container = Microsoft.Azure.Cosmos.Container;

namespace DatabaseLibrary
{
    public class DatabaseClient
    {
        private string _endPointUri { get; set; }
        private string _primaryKey { get; set; }

        private Database? _database;
        private Container? _container;
        private CosmosClient _cosmosClient;
        private string _containerId { get; set;}
        private string _databaseId { get; set; }
        private string _applicationName { get; set; }

        public DatabaseClient(string endPointUri, string primaryKey, string containerId, string databaseId, string applicationName)
        {
            _endPointUri = endPointUri;
            _primaryKey = primaryKey;
            _containerId = containerId;
            _databaseId = databaseId;
            _applicationName = applicationName;
            _cosmosClient = new CosmosClient(endPointUri, primaryKey, new CosmosClientOptions() { ApplicationName = applicationName });
            _container = _cosmosClient.GetContainer(databaseId, containerId);
        }
        
        public async Task AddItemToContainerAsync(SantaLetter letter)
        {
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<SantaLetter> letterResponse = await _container.ReadItemAsync<SantaLetter>(letter.Id, new PartitionKey(letter.PartitionKey));                
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<SantaLetter> andersenFamilyResponse = await _container.CreateItemAsync<SantaLetter>(letter, new PartitionKey(letter.PartitionKey));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
            }
        }

        public async Task<SantaLetter> QueryItemsByIdAsync(string Id, string partitionKey)
        {            
            ItemResponse<SantaLetter> itemResponse = await _container.ReadItemAsync<SantaLetter>(Id, new PartitionKey(partitionKey));

            return itemResponse;
        }

        public async Task<List<SantaLetter>> GetUnreadLetters()
        {
            string sqlQueryText = "SELECT * FROM c WHERE c.IsRead = false";

            return await QueryItemsAsync(sqlQueryText);
        }

        private async Task<List<SantaLetter>> QueryItemsAsync(string sqlQueryText)
        {
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<SantaLetter> queryResultSetIterator = _container.GetItemQueryIterator<SantaLetter>(queryDefinition);

            List<SantaLetter> letters = new List<SantaLetter>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<SantaLetter> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (SantaLetter family in currentResultSet)
                {
                    letters.Add(family);
                }
            }

            return letters;
        }
    }
}