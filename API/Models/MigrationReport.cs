using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace API.Models
{
    public class MigrationReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("migration_start_time")]
        public string? MigrationStartTime { get; set; }

        [BsonElement("sub_job_id")]
        public string? SubJobId { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("type")]
        public string? Type { get; set; }

        [BsonElement("source_id")]
        public string? SourceId { get; set; }

        [BsonElement("source")]
        public string? Source { get; set; }

        [BsonElement("destination_id")]
        public string? DestinationId { get; set; }

        [BsonElement("destination")]
        public string? Destination { get; set; }

        [BsonElement("size")]
        public string? Size { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }

        [BsonElement("migration_action")]
        public string? MigrationAction { get; set; }

        [BsonElement("comment")]
        public string? Comment { get; set; }

        [BsonElement("error_code")]
        public string? ErrorCode { get; set; }

        [BsonElement("hash")]
        public string? Hash { get; set; }

        [BsonIgnore]
        public string? GlobalSearch { get; set; }
    }
}
