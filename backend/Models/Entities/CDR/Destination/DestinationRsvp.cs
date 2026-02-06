using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class DestinationRsvp
    {
        [BsonElement("audio_stat")]
        [Display(Name = "Audio Status")]
        public RsvpStat? AudioStat { get; set; }

        [BsonElement("video_stat")]
        [Display(Name = "Video Status")]
        public RsvpStat? VideoStat { get; set; }
    }
}