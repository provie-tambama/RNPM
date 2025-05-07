using System.Collections.Generic;

namespace RNPM.Common.Models;

public class NetworkRequest : BaseEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    public string ApplicationId { get; set; }
    
    public virtual Application Application { get; set; }
    
    public virtual ICollection<HttpRequestInstance> HttpRequestInstances { get; set; }
}