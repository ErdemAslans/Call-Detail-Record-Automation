using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class FinalCalledParty
    {   
        [BsonElement("number")]
        [Display(Name = "Final Called Party Number")]
        public string? Number { get; set; }

        [BsonElement("partition")]
        [Display(Name = "Final Called Party Partition")]
        public string? Partition { get; set; }

        [BsonElement("uri")]
        [Display(Name = "Final Called Party URI")]
        public string? Uri { get; set; }

        [BsonElement("pattern")]
        [Display(Name = "Final Called Party Pattern")]
        public string? Pattern { get; set; }

        [BsonElement("unicode_login_user_id")]
        [Display(Name = "Final Called Party Unicode Login User ID")]
        public string? UnicodeLoginUserId { get; set; }
    }
}