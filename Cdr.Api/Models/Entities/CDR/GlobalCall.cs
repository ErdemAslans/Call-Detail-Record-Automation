using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class GlobalCall
    {   
        [BsonElement("call_id")]
        [Display(Name = "GlobalCall ID")]
        public string? CallId { get; set; }

        [BsonElement("manager_id")]
        [Display(Name = "GlobalCall Manager ID")]
        public int? ManagerId { get; set; }

        [BsonElement("cluster_id")]
        [Display(Name = "GlobalCall Cluster ID")]
        public string? ClusterId { get; set; }
    }
}