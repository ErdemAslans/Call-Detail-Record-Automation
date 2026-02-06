using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OriginalCalledParty
    {
        [BsonElement("number")]
        [Display(Name = "Original Called Party Number")]
        public string? Number { get; set; }

        [BsonElement("partition")]
        [Display(Name = "Original Called Party Partition")]
        public string? Partition { get; set; }

        [BsonElement("uri")]
        [Display(Name = "Original Called Party URI")]
        public string? Uri { get; set; }

        [BsonElement("pattern")]
        [Display(Name = "Original Called Party Pattern")]
        public string? Pattern { get; set; }
    }
}