using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr;

public class Authorization
{
    [BsonElement("level")]
    [Display(Name = "Authorization Level")]
    public int? Level { get; set; }

    [BsonElement("code_value")]
    [Display(Name = "Authorization Code Value")]
    public string? CodeValue { get; set; }
}