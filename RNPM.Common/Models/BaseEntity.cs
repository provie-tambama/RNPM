using System;

namespace RNPM.Common.Models;

public class BaseEntity
{
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string CreatorId { get; set; }
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
    public string ModifiedBy { get; set; }
    public bool IsArchived { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public bool IsActive { get; set; } = true;
}