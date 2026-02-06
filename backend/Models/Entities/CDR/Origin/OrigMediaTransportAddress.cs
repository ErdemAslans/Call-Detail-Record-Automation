using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OrigMediaTransportAddress
    {
        [Display(Name = "Origination Media Transport Address IP")]
        public string? Ip { get; set; }

        [Display(Name = "Origination Media Transport Address Port")]
        public int? Port { get; set; }
    }
}