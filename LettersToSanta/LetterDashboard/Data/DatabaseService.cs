using DatabaseLibrary;
using DatabaseLibrary.Models;

namespace LetterDashboard.Data
{
    public class DatabaseService
    {
        private DatabaseClient? _dbClient { get; set; }
        private readonly IConfiguration _configuration;

        public DatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbClient = new DatabaseClient(_configuration["CosmosEndpointUri"], _configuration["CosmosPrimaryKey"],
            _configuration["CosmosLetterContainerId"], _configuration["CosmosLetterDatabaseId"],
            _configuration["FunctionApplicationName"]);
        }   
        
        public async Task<List<SantaLetter>> GetLetters()
        {
            return await _dbClient.GetUnreadLetters();
        }

        public async Task<SantaLetter> GetLetter(string id, string partitionKey)
        {
            return await _dbClient.QueryItemsByIdAsync(id, partitionKey);
        }
    }
}
