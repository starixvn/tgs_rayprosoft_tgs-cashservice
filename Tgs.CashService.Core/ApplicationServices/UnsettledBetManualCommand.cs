using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record UnsettledBetManualCommand(
       Guid WalletId,
       decimal Amount,
       string Description,
       string ExternalId,
       Guid CreatedByUserId);
}
