using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class HuntPilot
    {
        [BsonElement("dn")]
        [Display(Name = "Hunt Pilot DN")]
        public string? Dn { get; set; }

        [BsonElement("partition")]
        [Display(Name = "Hunt Pilot Partition")]
        public string? Partition { get; set; }

        [BsonElement("pattern")]
        [Display(Name = "Hunt Pilot Pattern")]
        public string? Pattern { get; set; }
    }
}