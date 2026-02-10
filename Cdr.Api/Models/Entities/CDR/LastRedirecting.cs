using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class LastRedirecting
    {
        [BsonElement("party_pattern")]
        [Display(Name = "Last Redirecting Party Pattern")]
        public string? PartyPattern { get; set; }

        [BsonElement("routing_reason")]
        [Display(Name = "Last Redirecting Routing Reason")]
        public int? RoutingReason { get; set; }
    }
}