using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OutPulsed
    {
        [BsonElement("called_party_number")]
        [Display(Name = "Outgoing Pulsed Called Party Number")]
        public string? CalledPartyNumber { get; set; }

        [BsonElement("calling_party_number")]
        [Display(Name = "Outgoing Pulsed Calling Party Number")]
        public string? CallingPartyNumber { get; set; }

        [BsonElement("last_redirecting_number")]
        [Display(Name = "Outgoing Pulsed Last Redirecting Number")]
        public string? LastRedirectingNumber { get; set; }

        [BsonElement("original_called_party_number")]
        [Display(Name = "Outgoing Pulsed Original Called Party Number")]
        public string? OriginalCalledPartyNumber { get; set; }
    }
}