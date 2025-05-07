using RNPM.Common.Interfaces;

namespace RNPM.APIServices
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}
