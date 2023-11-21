using NSubstitute;
using Tgs.CashServiceCommitter.ApplicationServices;

namespace Tgs.CashServiceCommitter.Core.UnitTests;

public class UpdateWalletCommandServiceTests
{
    [Fact]
    public async Task 更新資料庫錢包到最新狀態()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new UpdateWalletCommandService(fakeTgsRepository);

        var command = new UpdateWalletCommand(Guid.NewGuid());

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakeTgsRepository.Received(1)
            .UpdateWalletAsync(Arg.Is(command.WalletId));
    }
}
