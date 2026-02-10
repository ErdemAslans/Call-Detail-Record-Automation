using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class LastRedirect
    {
        [BsonElement("dn")]
        [Display(Name = "Last Redirect DN")]
        public string? Dn { get; set; }

        [BsonElement("dn_partition")]
        [Display(Name = "Last Redirect DN Partition")]
        public string? DnPartition { get; set; }

        [BsonElement("uri")]
        [Display(Name = "Last Redirect URI")]
        public string? Uri { get; set; }

        [BsonElement("redirect_on_behalf_of")]
        [Display(Name = "Last Redirect On Behalf Of")]
        public OnBehalfOfCode? RedirectOnBehalfOf { get; set; }

        [BsonElement("reason")]
        [Display(Name = "Last Redirect Reason")]
        public RedirectReason? Reason { get; set; }
    }
}