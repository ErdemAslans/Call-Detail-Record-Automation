using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class DestinationMediaTransportAddress
    {
        [BsonElement("ip")]
        [Display(Name = "Destination Media Transport Address IP")]
        public string? Ip { get; set; }

        [BsonElement("port")]
        [Display(Name = "Destination Media Transport Address Port")]
        public int? Port { get; set; }

        [BsonElement("ip_channel2")]
        [Display(Name = "Destination Media Transport Address IP Channel 2")]
        public string? IpChannel2 { get; set; }

        [BsonElement("port_channel2")]
        [Display(Name = "Destination Media Transport Address Port Channel 2")]
        public int? PortChannel2 { get; set; }
    }
}