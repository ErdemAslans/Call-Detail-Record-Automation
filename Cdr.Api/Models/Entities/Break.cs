using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Cdr.Api.Models.Entities
{
    public class Break
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string UserId { get; set; }

        [BsonElement("startTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("endTime")]
        public DateTime? EndTime { get; set; }

        [BsonElement("plannedEndTime")]
        public DateTime PlannedEndTime { get; set; }

        [BsonElement("reason")]
        public string? Reason { get; set; }

        [BsonElement("user")]
        [BsonIgnoreIfNull]
        public Operator User { get; set; }
    }
}
