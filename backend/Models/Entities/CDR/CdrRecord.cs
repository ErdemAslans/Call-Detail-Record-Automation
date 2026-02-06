using System.ComponentModel.DataAnnotations;
using Cdr.Api.Models.Entities;
using Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using YourNamespace;

namespace Cdr.Api.Entities.Cdr
{
    public class CdrRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("authCodeDescription")]
        [Display(Name = "Auth Code Description")]
        public string? AuthCodeDescription { get; set; }

        [BsonElement("authorization")]
        [Display(Name = "Authorization")]
        public Authorization? Authorization { get; set; }

        [BsonElement("callSecuredStatus")]
        [Display(Name = "Call Secured Status")]
        public int? CallSecuredStatus { get; set; }

        [BsonElement("calledPartyPatternUsage")]
        [Display(Name = "Called Party Pattern Usage")]
        public CalledPartyPatternUsage? CalledPartyPatternUsage { get; set; }

        [BsonElement("callingParty")]
        [Display(Name = "Calling Party")]
        public CallingParty? CallingParty { get; set; }

        [BsonElement("cdrRecordType")]
        [Display(Name = "CDR Record Type")]
        public CdrRecordType? CdrRecordType { get; set; }

        [BsonElement("clientMatterCode")]
        [Display(Name = "Client Matter Code")]
        public string? ClientMatterCode { get; set; }

        [BsonElement("comment")]
        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        [BsonElement("currentRoutingReason")]
        [Display(Name = "Current Routing Reason")]
        public int? CurrentRoutingReason { get; set; }

        [BsonElement("dateTime")]
        [Display(Name = "Date Time")]
        public DateTimeInfo? DateTime { get; set; }

        [BsonElement("destination")]
        [Display(Name = "Destination")]
        public Destination? Destination { get; set; }

        [BsonElement("duration")]
        [Display(Name = "Duration")]
        public int? Duration { get; set; }

        [BsonElement("finalCalledParty")]
        [Display(Name = "Final Called Party Number")]
        public FinalCalledParty? FinalCalledParty { get; set; }

        [BsonElement("finalMobileCalledPartyNumber")]
        [Display(Name = "Final Mobile Called Party Number")]
        public string? FinalMobileCalledPartyNumber { get; set; }

        [BsonElement("globalCall")]
        [Display(Name = "Global Call")]
        public GlobalCall? GlobalCall { get; set; }

        [BsonElement("huntPilot")]
        [Display(Name = "Hunt Pilot")]
        public HuntPilot? HuntPilot { get; set; }

        [BsonElement("incoming")]
        [Display(Name = "Incoming")]
        public Incoming? Incoming { get; set; }

        [BsonElement("joinBehalfOf")]
        [Display(Name = "Join Behalf Of")]
        public OnBehalfOfCode? JoinBehalfOf { get; set; }

        [BsonElement("lastRedirect")]
        [Display(Name = "Last Redirect")]
        public LastRedirect? LastRedirect { get; set; }

        [BsonElement("lastRedirecting")]
        [Display(Name = "Last Redirecting")]
        public LastRedirecting? LastRedirecting { get; set; }

        [BsonElement("mobileCallingPartyNumber")]
        [Display(Name = "Mobile Calling Party Number")]
        public string? MobileCallingPartyNumber { get; set; }

        [BsonElement("mobileCallType")]
        [Display(Name = "Mobile Call Type")]
        public int? MobileCallType { get; set; }

        [BsonElement("orig")]
        [Display(Name = "Orig")]
        public Orig? Orig { get; set; }

        [BsonElement("originalCalledParty")]
        [Display(Name = "Original Called Party")]
        public OriginalCalledParty? OriginalCalledParty { get; set; }

        [BsonElement("outgoing")]
        [Display(Name = "Outgoing")]
        public Outgoing? Outgoing { get; set; }

        [BsonElement("outpulsed")]
        [Display(Name = "Outpulsed")]
        public OutPulsed? Outpulsed { get; set; }

        [BsonElement("pk_id")]
        [Display(Name = "PKID")]
        public string? Pkid { get; set; }

        [BsonElement("wasCallQueued")]
        [Display(Name = "Was Call Queued")]
        public bool? WasCallQueued { get; set; }

        [BsonElement("totalWaitTimeInQueue")]
        [Display(Name = "Total Wait Time In Queue")]
        public int? TotalWaitTimeInQueue { get; set; }

        [BsonElement("callDirection")]
        [Display(Name = "Call Direction")]
        public CallDirection? CallDirection { get; set; }

        [BsonElement("operator")]
        [Display(Name = "Operator")]
        public Operator? Operator { get; set; }
    }
}