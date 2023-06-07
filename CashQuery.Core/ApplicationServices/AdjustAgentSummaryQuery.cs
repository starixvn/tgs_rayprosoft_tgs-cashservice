using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tgs.Cash.Common;

namespace Tgs.CashQueryService.ApplicationServices;
public record AdjustAgentSummaryQuery(
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    Guid? AgentId,
    string? Operator);
