using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.Models
{
    public class DepositViewModel
    {
        [Range(0, int.MaxValue)]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public string? ExternalId { get; set; }
    }
}
