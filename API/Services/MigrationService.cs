using API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace API.Services
{
    public class MigrationService
    {
        private readonly IMongoCollection<MigrationReport> _migrationCollection;

        public MigrationService(IConfiguration config)
        {
            var mongoClient = new MongoClient(config["MONGODB_CONNECTION_STRING"]);
            var mongoDatabase = mongoClient.GetDatabase(config["MONGODB_DATABASE_NAME"]);

            _migrationCollection = mongoDatabase.GetCollection<MigrationReport>("MigrationReports");
        }

        public async Task CreateAsync(MigrationReport newReport) =>
            await _migrationCollection.InsertOneAsync(newReport);

        public async Task CreateManyAsync(List<MigrationReport> newReports) =>
            await _migrationCollection.InsertManyAsync(newReports);

        public async Task<List<MigrationReport>> GetAsync() =>
            await _migrationCollection.Find(_ => true).ToListAsync();

        public async Task<List<MigrationReport>> SearchAsync(string? title, string? status)
        {
            var builder = Builders<MigrationReport>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(title))
            {
                filter &= builder.Regex("title", new BsonRegularExpression(title, "i"));
            }

            if (!string.IsNullOrEmpty(status))
            {
                filter &= builder.Eq("status", status);
            }

            return await _migrationCollection.Find(filter).ToListAsync();
        }
    }
}
