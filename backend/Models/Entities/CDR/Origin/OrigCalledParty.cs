using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OrigCalledParty
    {
        [BsonElement("redirect_on_behalf_of")]
        [Display(Name = "Origination Call Party Redirect On Behalf Of")]
        public OnBehalfOfCode? RedirectOnBehalfOf { get; set; }
        
        [BsonElement("redirect_reason")]
        [Display(Name = "Origination Call Party Redirect Reason")]
        public int? RedirectReason { get; set; }
    }
}