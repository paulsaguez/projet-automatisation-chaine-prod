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

        public async Task<List<MigrationReport>> SearchAsync(MigrationReport filter)
        {
            var builder = Builders<MigrationReport>.Filter;
            var mongoFilter = builder.Empty;

            if (!string.IsNullOrEmpty(filter.Title)) mongoFilter &= builder.Regex("title", new BsonRegularExpression(filter.Title, "i"));
            if (!string.IsNullOrEmpty(filter.Status)) mongoFilter &= builder.Regex("status", new BsonRegularExpression(filter.Status, "i"));
            if (!string.IsNullOrEmpty(filter.Source)) mongoFilter &= builder.Regex("source", new BsonRegularExpression(filter.Source, "i"));
            if (!string.IsNullOrEmpty(filter.Destination)) mongoFilter &= builder.Regex("destination", new BsonRegularExpression(filter.Destination, "i"));
            if (!string.IsNullOrEmpty(filter.Type)) mongoFilter &= builder.Regex("type", new BsonRegularExpression(filter.Type, "i"));
            if (!string.IsNullOrEmpty(filter.MigrationAction)) mongoFilter &= builder.Regex("migration_action", new BsonRegularExpression(filter.MigrationAction, "i"));
            if (!string.IsNullOrEmpty(filter.SubJobId)) mongoFilter &= builder.Regex("sub_job_id", new BsonRegularExpression(filter.SubJobId, "i"));
            if (!string.IsNullOrEmpty(filter.SourceId)) mongoFilter &= builder.Regex("source_id", new BsonRegularExpression(filter.SourceId, "i"));
            if (!string.IsNullOrEmpty(filter.DestinationId)) mongoFilter &= builder.Regex("destination_id", new BsonRegularExpression(filter.DestinationId, "i"));
            if (!string.IsNullOrEmpty(filter.ErrorCode)) mongoFilter &= builder.Regex("error_code", new BsonRegularExpression(filter.ErrorCode, "i"));
            if (!string.IsNullOrEmpty(filter.MigrationStartTime)) mongoFilter &= builder.Regex("migration_start_time", new BsonRegularExpression(filter.MigrationStartTime, "i"));

            return await _migrationCollection.Find(mongoFilter).ToListAsync();
        }
    }
}
