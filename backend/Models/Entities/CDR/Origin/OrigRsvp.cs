using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OrigRsvp
    {   
        [BsonElement("audio_stat")]
        [Display(Name = "Origination Audio Status")]
        public RsvpStat? AudioStat { get; set; }

        [BsonElement("video_stat")]
        [Display(Name = "Origination Video Status")]
        public RsvpStat? VideoStat { get; set; }
    }
}