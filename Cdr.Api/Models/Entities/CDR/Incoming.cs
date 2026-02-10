using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class Incoming
    {
        [BsonElement("icid")]
        [Display(Name = "Incoming Call ID")]
        public string? Icid { get; set; }

        [BsonElement("orig_ioi")]
        [Display(Name = "Incoming Call Orig IOI")]
        public string? OrigIoi { get; set; }

        [BsonElement("protocol_call_ref")]
        [Display(Name = "Incoming Call Protocol Call Reference")]
        public string? ProtocolCallRef { get; set; }

        [BsonElement("protocol_id")]
        [Display(Name = "Incoming Call Protocol ID")]
        public Protocol? ProtocolId { get; set; }

        [BsonElement("term_ioi")]
        [Display(Name = "Incoming Call Term IOI")]
        public string? TermIoi { get; set; }
    }
}