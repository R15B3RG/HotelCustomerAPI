using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HotelCustomerAPI.HotelDbContext
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> options)
        {
            var mongoDbSettings = options.Value;

            if (string.IsNullOrEmpty(mongoDbSettings.ConnectionString))
            {
                throw new ArgumentNullException(nameof(mongoDbSettings.ConnectionString), "MongoDB connection string is not configured.");
            }

            if (string.IsNullOrEmpty(mongoDbSettings.DatabaseName))
            {
                throw new ArgumentNullException(nameof(mongoDbSettings.DatabaseName), "MongoDB database name is not configured.");
            }

            var client = new MongoClient(mongoDbSettings.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            var settings = new MongoCollectionSettings
            {
                AssignIdOnInsert = true
            };

            return _database.GetCollection<T>(collectionName, settings);
        }
    }
}
