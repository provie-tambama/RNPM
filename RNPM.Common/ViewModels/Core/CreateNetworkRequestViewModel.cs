namespace RNPM.Common.ViewModels.Core;

public class CreateNetworkRequestViewModel
{
    public required string UniqueAccessCode { get; set; }
    public required string Name { get; set; }
    public Decimal RequestCompletionTime { get; set; }
}