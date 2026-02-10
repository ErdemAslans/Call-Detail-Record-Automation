using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OrigVideoCap
    {
        [BsonElement("bandwidth")]
        [Display(Name = "Origination Video Bandwidth")]
        public int? Bandwidth { get; set; }

        [BsonElement("bandwidth_channel2")]
        [Display(Name = "Origination Video Bandwidth Channel 2")]
        public int? BandwidthChannel2 { get; set; }

        [BsonElement("codec")]
        [Display(Name = "Origination Video Codec")]
        public int? Codec { get; set; }

        public string? CodecDescription => Codec switch
        {
            >= 0 and <= 100 => "H.261",
            101 => "H.263",
            103 => "H.264",
            _ => null
        };

        [BsonElement("codec_channel2")]
        [Display(Name = "Origination Video Codec Channel 2")]
        public int? CodecChannel2 { get; set; }

        public string? CodecChannel2Description => CodecChannel2 switch
        {
            >= 0 and <= 100 => "H.261",
            101 => "H.263",
            103 => "H.264",
            _ => null
        };

        [BsonElement("resolution")]
        [Display(Name = "Origination Video Resolution")]
        public VideoCapResolution? Resolution { get; set; }

        [BsonElement("resolution_channel2")]
        [Display(Name = "Origination Video Resolution Channel 2")]
        public VideoCapResolution? ResolutionChannel2 { get; set; }
    }
}