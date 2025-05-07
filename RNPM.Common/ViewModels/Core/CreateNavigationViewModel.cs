namespace RNPM.Common.ViewModels.Core;

public class CreateNavigationViewModel
{
    public required string UniqueAccessCode { get; set; }
    public required string FromScreen { get; set; }
    public required string ToScreen { get; set; }
    public required Decimal NavigationCompletionTime { get; set; }
}