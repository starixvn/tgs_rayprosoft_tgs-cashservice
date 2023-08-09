namespace Tgs.CashQueryService.Client;
public record AdjustRecordListItem(
    Guid Id,
    Guid BrandId,
    string UserId,
    AdjustType AdjustType,
    OperationMode OperationMode,
    decimal Amount,
    decimal BucketAmount,
    string Operator,
    string? Remark,
    DateTime CreatedOn,
    UserType UserType = UserType.None);
