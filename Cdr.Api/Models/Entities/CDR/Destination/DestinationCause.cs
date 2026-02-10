using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class DestinationCause
    {
        [BsonElement("location")]
        [Display(Name = "Destination Cause Location")]
        public int? Location { get; set; }

        [BsonElement("value")]
        [Display(Name = "Destination Cause Value")]
        public CauseValue? Value { get; set; }
    }
}