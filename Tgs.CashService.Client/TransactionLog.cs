using System.Text.Json.Serialization;

namespace Tgs.CashService.Client;
public record TransactionLog(
    Guid Id,
    DateTime RecordedOn,
    [property: JsonPropertyName("type")] int TransactionType,
    decimal Amount,
    decimal Balance,
    decimal LockAmount,
    decimal LockedBalance,
    string? Description,
    string? ExternalId);
