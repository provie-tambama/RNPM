using System.ComponentModel;

namespace RNPM.Common.Data.Enums;

public enum MetricStatus
{
    [Description("Excellent")]
    Excellent,
    [Description("Good")]
    Good,
    [Description("Acceptable")]
    Acceptable,
    [Description("Needs Improvement")]
    NeedsImprovement,
    [Description("Poor")]
    Poor,
    [Description("Very Poor")]
    VeryPoor
    
}