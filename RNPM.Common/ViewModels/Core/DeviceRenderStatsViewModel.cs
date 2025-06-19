// RNPM.Common/ViewModels/Core/DeviceRenderStatsViewModel.cs
namespace RNPM.Common.ViewModels.Core
{
    public class DeviceRenderStatsViewModel
    {
        public string DeviceBrand { get; set; }
        public string DeviceModel { get; set; }
        public int RenderCount { get; set; }
        public double AverageRenderTime { get; set; }
    }
}

// RNPM.Common/ViewModels/Core/OsRenderStatsViewModel.cs
namespace RNPM.Common.ViewModels.Core
{
    public class OsRenderStatsViewModel
    {
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public int RenderCount { get; set; }
        public double AverageRenderTime { get; set; }
    }
}