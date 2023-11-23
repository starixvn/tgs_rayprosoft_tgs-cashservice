using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.CashService;

namespace Tgs.CashServiceCommitter.Repositories;
internal class TestRepository : ITgsRepository, ITransactionLogRepository, IDistributedResourceManager
{
    public ValueTask InsertTransactionLogsAsync(IEnumerable<TransactionRecord> records, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ValueTask MigrateTransactionLogsAsync(IEnumerable<TransactionRecord> records, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ValueTask UpdateWalletAsync(Guid walletId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public IAsyncEnumerable<TransactionRecord> GetWaitingBookkeepLogsAsync(Guid walletId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ValueTask RemoveBookkeepLogsAsync(Guid walletId, int length, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ValueTask<string> BidAsync(Guid walletId, string token, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ValueTask<string?> GetBidTokenAsync(Guid walletId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ValueTask DiscardBidAsync(Guid walletId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ValueTask ExtendBidAsync(Guid walletId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
