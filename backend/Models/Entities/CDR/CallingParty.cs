using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr;

public class CallingParty
{
    [BsonElement("number")]
    [Display(Name = "Calling Party Number")]
    public string? Number { get; set; }

    [BsonElement("partition")]
    [Display(Name = "Calling Party Partition")]
    public string? Partition { get; set; }

    [BsonElement("uri")]
    [Display(Name = "Calling Party URI")]
    public string? Uri { get; set; }

    [BsonElement("unicode_login_user_id")]
    [Display(Name = "Calling Party Unicode Login User ID")]
    public string? UnicodeLoginUserId { get; set; }
}