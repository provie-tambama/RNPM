namespace RNPM.Common.ViewModels.Core
{
    public class DeviceDistributionViewModel
    {
        public List<DeviceTypeCount> DeviceTypes { get; set; }
        public List<OsVersionCount> OsVersions { get; set; }
        public List<DeviceModelCount> DeviceModels { get; set; }
    }

    public class DeviceTypeCount
    {
        public string DeviceType { get; set; }
        public int Count { get; set; }
        public double AverageTime { get; set; }
    }

    public class OsVersionCount
    {
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public int Count { get; set; }
        public double AverageTime { get; set; }
    }

    public class DeviceModelCount
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Count { get; set; }
        public double AverageTime { get; set; }
    }
}