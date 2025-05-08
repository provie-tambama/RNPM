using System;
using RNPM.Common.Interfaces;

namespace RNPM.Common.Services;

public class DateTimeService: IDateTimeService
{
    public DateTime Now => DateTime.Now;
}
