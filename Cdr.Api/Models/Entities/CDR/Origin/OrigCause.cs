using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OrigCause
    {
        [BsonElement("location")]
        [Display(Name = "Origination Cause Location")]
        public int? Location { get; set; }

        [BsonElement("value")]
        [Display(Name = "Origination Cause Value")]
        public CauseValue? Value { get; set; }
    }
}