using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record BalanceService : IGetService<BalanceQuery, decimal?>
    {
        private readonly ILogger<BalanceService> _logger;

        private readonly IWalletRepository _walletRepository;

        public BalanceService(ILogger<BalanceService> logger, IWalletRepository walletRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public ValueTask<decimal?> GetAsync(BalanceQuery query, CancellationToken cancellationToken = default)
        => _walletRepository.GetBalanceAsync(query.WalletId);
    }
}
