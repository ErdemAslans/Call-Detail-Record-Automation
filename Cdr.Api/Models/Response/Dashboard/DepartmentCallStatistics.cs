using MongoDB.Bson.Serialization.Attributes;

namespace Cdr.Api.Models.Response;

public class DepartmentCallStatistics
{
    [BsonElement("departmentName")]
    public required string DepartmentName { get; set; }

    [BsonElement("totalCalls")]
    public int TotalCalls { get; set; }

    [BsonElement("answeredCalls")]
    public int AnsweredCalls { get; set; }

    [BsonElement("missedCalls")]
    public int MissedCalls { get; set; }

    [BsonElement("onBreakCalls")]
    public int OnBreakCalls { get; set; }

    [BsonElement("answeredCallRate")]
    public double AnsweredCallRate { get; set; }
} 