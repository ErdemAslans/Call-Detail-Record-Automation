using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class Orig
    {
        [BsonElement("call_termination_on_behalf_of")]
        [Display(Name = "Origination Call Termination On Behalf Of")]
        public OnBehalfOfCode? CallTerminationOnBehalfOf { get; set; }

        [BsonElement("call_party")]
        [Display(Name = "Origination Call Party")]
        public OrigCalledParty? CallParty { get; set; }

        [BsonElement("cause")]
        [Display(Name = "Origination Cause")]
        public OrigCause? Cause { get; set; }

        [BsonElement("conversation_id")]
        [Display(Name = "Origination Conversation ID")]
        public int? ConversationId { get; set; }

        [BsonElement("dtmf_method")]
        [Display(Name = "Origination DTMF Method")]
        public int? DtmfMethod { get; set; }

        [BsonElement("device_name")]
        [Display(Name = "Origination Device Name")]
        public string? DeviceName { get; set; }

        [BsonElement("ip_addr")]
        [Display(Name = "Origination IP Address")]
        public string? IpAddr { get; set; }

        [BsonElement("ipv4v6_addr")]
        [Display(Name = "Origination IPv4v6 Address")]
        public string? Ipv4v6Addr { get; set; }

        [BsonElement("leg_call_identifier")]
        [Display(Name = "Origination Leg Call Identifier")]
        public int? LegCallIdentifier { get; set; }

        [BsonElement("orig_media_cap")]
        [Display(Name = "Origination Media Capability")]
        public OrigMediaCap? OrigMediaCap { get; set; }

        [BsonElement("node_id")]
        [Display(Name = "Origination Node ID")]
        public int? NodeId { get; set; }

        [BsonElement("precedence_level")]
        [Display(Name = "Origination Precedence Level")]
        public PrecedenceLevel? PrecedenceLevel { get; set; }

        [BsonElement("rsvp")]
        [Display(Name = "Origination RSVP")]
        public OrigRsvp? Rsvp { get; set; }

        [BsonElement("routing_reason")]
        [Display(Name = "Origination Routing Reason")]
        public int? RoutingReason { get; set; }

        [BsonElement("span")]
        [Display(Name = "Origination Span")]
        public int? Span { get; set; }

        [BsonElement("video_cap")]
        [Display(Name = "Origination Video Capability")]
        public OrigVideoCap? VideoCap { get; set; }

        [BsonElement("video_channel_role_channel2")]
        [Display(Name = "Orig Video Channel Role Channel 2")]
        public VideoChannelRoleChannel2? VideoChannelRoleChannel2 { get; set; }
        
        [BsonElement("video_transport_address")]
        [Display(Name = "Origination Video Transport Address")]
        public OrigVideoTransportAddress? VideoTransportAddress { get; set; }
    }
}