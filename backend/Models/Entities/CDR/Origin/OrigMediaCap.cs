using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OrigMediaCap
    {
        [BsonElement("bandwidth")]
        [Display(Name = "Origination Media Bandwidth")]
        public int? Bandwidth { get; set; }

        [BsonElement("bandwidth_channel2")]
        [Display(Name = "Origination Media Bandwidth Channel 2")]
        public int? BandwidthChannel2 { get; set; }

        [BsonElement("g723_bit_rate")]
        [Display(Name = "Origination Media G723 Bit Rate")]
        public int? G723BitRate { get; set; }

        [BsonElement("max_frames_per_packet")]
        [Display(Name = "Origination Media Max Frames Per Packet")]
        public int? MaxFramesPerPacket { get; set; }

        [BsonElement("payload_capability")]
        [Display(Name = "Origination Media Payload Capability")]
        public int? PayloadCapability { get; set; }
    }
}