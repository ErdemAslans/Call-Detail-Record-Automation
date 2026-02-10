using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class DestinationMediaCap
    {   
        [BsonElement("bandwidth")]
        [Display(Name = "Destination Media Bandwidth")]
        public int? Bandwidth { get; set; }

        [BsonElement("bandwidth_channel2")]
        [Display(Name = "Destination Media Bandwidth Channel 2")]
        public int? BandwidthChannel2 { get; set; }

        [BsonElement("g723_bit_rate")]
        [Display(Name = "Destination Media G723 Bit Rate")]
        public int? G723BitRate { get; set; }

        [BsonElement("max_frames_per_packet")]
        [Display(Name = "Destination Media Max Frames Per Packet")]
        public int? MaxFramesPerPacket { get; set; }
        
        [BsonElement("payload_capacity")]
        [Display(Name = "Destination Media Payload Capability")]
        public int? PayloadCapacity { get; set; }
    }
}