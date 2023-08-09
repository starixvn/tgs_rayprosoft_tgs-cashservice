namespace Tgs.CashQueryService;
public record AdjustRecord(
    Guid Id,
    Guid BrandId,
    string UserId,
    UserType UserType,
    AdjustType AdjustType,
    AdjustmentType AdjustmentType,
    decimal Amount,
    decimal BucketAmount,
    string Operator,
    string? Remark,
    DateTime CreatedOn);
