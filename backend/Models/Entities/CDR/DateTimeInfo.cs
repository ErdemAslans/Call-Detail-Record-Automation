// Purpose: Model for DateTimeInfo entity in CDR context.
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr;
public class DateTimeInfo
{
    [BsonElement("connect")]
    [Display(Name = "Connect Time")]
    public DateTime? Connect { get; set; }

    [BsonElement("disconnect")]
    [Display(Name = "Disconnect Time")]
    public DateTime? Disconnect { get; set; }

    [BsonElement("origination")]
    [Display(Name = "Origination Time")]
    public DateTime? Origination { get; set; }
}
