using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class DestinationDeviceMobile
    {
        [BsonElement("call_duration")]
        [Display(Name = "Call Duration")]
        public int? CallDuration { get; set; }

        [BsonElement("device_name")]
        [Display(Name = "Device Name")]
        public string? DeviceName { get; set; }
    }
}