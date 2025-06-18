using System;

namespace RNPM.Common.Models;

public class HttpRequestInstance : BaseEntity
{
    public string Id { get; set; }
    public string RequestId { get; set; }
    public DateTime RequestDate { get; set; }
    public Decimal RequestCompletionTime { get; set; }
    public string DeviceInfoId { get; set; }
    public virtual DeviceInfo DeviceInfo { get; set; }
    public virtual NetworkRequest NetworkRequest { get; set; }
}