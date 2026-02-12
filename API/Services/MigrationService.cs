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

            // Create unique index on Hash field
            var indexKeys = Builders<MigrationReport>.IndexKeys.Ascending(x => x.Hash);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<MigrationReport>(indexKeys, indexOptions);
            _migrationCollection.Indexes.CreateOne(indexModel);
        }

        public async Task CreateAsync(MigrationReport newReport) =>
            await _migrationCollection.InsertOneAsync(newReport);

        public async Task CreateManyAsync(List<MigrationReport> newReports)
        {
            try
            {
                // IsOrdered = false continues processing even if some inserts fail (e.g. duplicates)
                await _migrationCollection.InsertManyAsync(newReports, new InsertManyOptions { IsOrdered = false });
            }
            catch (MongoBulkWriteException ex)
            {
                // Ignore duplicate key errors (code 11000)
                // Log other errors if needed
                foreach (var error in ex.WriteErrors)
                {
                    if (error.Code != 11000)
                    {
                        Console.WriteLine($"Write error: {error.Message}");
                    }
                }
            }
        }

        public async Task<List<MigrationReport>> GetAsync() =>
            await _migrationCollection.Find(_ => true).ToListAsync();

        public async Task<List<MigrationReport>> SearchAsync(MigrationReport filter)
        {
            var builder = Builders<MigrationReport>.Filter;
            
            // Initial filter (empty -> match all)
            var mongoFilter = builder.Empty;

            // Global Search Logic
            if (!string.IsNullOrEmpty(filter.GlobalSearch))
            {
                var searchRegex = new BsonRegularExpression(filter.GlobalSearch, "i");
                mongoFilter &= builder.Or(
                    builder.Regex("title", searchRegex),
                    builder.Regex("status", searchRegex),
                    builder.Regex("source", searchRegex),
                    builder.Regex("destination", searchRegex),
                    builder.Regex("type", searchRegex),
                    builder.Regex("migration_action", searchRegex),
                    builder.Regex("sub_job_id", searchRegex),
                    builder.Regex("source_id", searchRegex),
                    builder.Regex("destination_id", searchRegex),
                    builder.Regex("error_code", searchRegex),
                    builder.Regex("comment", searchRegex)
                );
            }

            // Specific Field Filters (combine with Global Search if both exist)
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

        public async Task<List<string>> GetExistingHashesAsync(List<string> hashes)
        {
            var filter = Builders<MigrationReport>.Filter.In(x => x.Hash, hashes);
            var projection = Builders<MigrationReport>.Projection.Include(x => x.Hash);
            
            var result = await _migrationCollection.Find(filter)
                .Project(projection)
                .ToListAsync();

            // Extract just the hash strings (handling possible nulls though Hash is indexed)
            return result.Select(x => x.GetElement("hash").Value.AsString).ToList();
        }
    }
}
