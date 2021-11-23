using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.ViewModels
{
    public class RebateViewModel
    {
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public string? ExternalId { get; set; }
    }
}
