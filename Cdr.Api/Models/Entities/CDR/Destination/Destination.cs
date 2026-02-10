using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class Destination
    {
        [BsonElement("call_termination_on_behalf_of")]
        [Display(Name = "Call Termination On Behalf Of")]
        public OnBehalfOfCode? CallTerminationOnBehalfOf { get; set; }

        [BsonElement("conversation_id")]
        [Display(Name = "Conversation ID")]
        public string? ConversationId { get; set; }

        [BsonElement("dtmf_method")]
        [Display(Name = "DTMF Method")]
        public string? DtmfMethod { get; set; }

        [BsonElement("device_name")]
        [Display(Name = "Device Name")]
        public string? DeviceName { get; set; }

        [BsonElement("ip_addr")]
        [Display(Name = "Destination IP Address")]
        public string? IpAddr { get; set; }

        [BsonElement("ipv4v6_addr")]
        [Display(Name = "Destination IPv4v6 Address")]
        public string? Ipv4v6Addr { get; set; }


        [BsonElement("leg_identifier")]
        [Display(Name = "Leg Identifier")]
        public int? LegIdentifier { get; set; }
        
        [BsonElement("cause")]
        [Display(Name = "Destination Cause")]
        public DestinationCause? Cause { get; set; }

        [BsonElement("media_cap")]
        [Display(Name = "Destination Media Capability")]
        public DestinationMediaCap? MediaCap { get; set; }

        [BsonElement("media_transport_address")]
        [Display(Name = "Destination Media Transport Address")]
        public DestinationMediaTransportAddress? MediaTransportAddress { get; set; }

        [BsonElement("mobile")]
        [Display(Name = "Device Mobile")]
        public DestinationDeviceMobile? Mobile { get; set; }

        [BsonElement("node_id")]
        [Display(Name = "Node ID")]
        public int? NodeId { get; set; }

        [BsonElement("precedence_level")]
        [Display(Name = "Precedence Level")]
        public PrecedenceLevel? PrecedenceLevel { get; set; }

        [BsonElement("rsvp")]
        [Display(Name = "Destination RSVP")]
        public DestinationRsvp? Rsvp { get; set; }

        [BsonElement("span")]
        [Display(Name = "Destination Span")]
        public int? Span { get; set; }

        [BsonElement("video_cap")]
        [Display(Name = "Destination Video Capability")]
        public DestinationVideoCap? VideoCap { get; set; }

        [BsonElement("video_channel_role_channel2")]
        [Display(Name = "Destination Video Channel Role Channel 2")]
        public VideoChannelRoleChannel2? VideoChannelRoleChannel2 { get; set; }

        [BsonElement("video_transport_address")]
        [Display(Name = "Destination Video Transport Address")]
        public DestinationVideoTransportAddress? VideoTransportAddress { get; set; }
    }
}