namespace RNPM.Common.ViewModels.Core;

public class ApplicationViewModel
{
    public  string Id { get; set; }
    public string? Name { get; set; }
    public string UniqueAccessCode { get; set; }
    public  DateTime CreatedDate { get; set; }
    public string? CreatorId { get; set; }
    public string? ModifiedBy { get; set; }
    public string? UserId { get; set; }
    
}