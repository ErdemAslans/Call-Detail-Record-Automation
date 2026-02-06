using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Entities.Cdr
{
    public class OrigMobile
    {
        [Display(Name = "Origination Call Duration")]
        public int? CallDuration { get; set; }

        [Display(Name = "Origination Device Name")]
        public string? DeviceName { get; set; }
    }
}