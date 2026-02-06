using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cdr.Api.Entities;

public class Department
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }
}