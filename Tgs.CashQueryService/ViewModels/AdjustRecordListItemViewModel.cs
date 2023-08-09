namespace Tgs.CashQueryService.ViewModels;

public class AdjustRecordListItemViewModel
{
    public required Guid Id { get; set; }

    public required DateTime CreatedOn { get; set; }

    public required Guid BrandId { get; set; }

    public required string UserId { get; set; }

    public required AdjustType AdjustType { get; set; }

    public required AdjustmentType AdjustmentType { get; set; }

    public required decimal Amount { get; set; }

    public required decimal BucketAmount { get; set; }

    public required string Operator { get; set; }

    public string? Remark { get; set; }
}
