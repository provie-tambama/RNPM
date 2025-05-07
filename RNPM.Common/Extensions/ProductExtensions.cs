using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RNPM.Common.Extensions;

public static class ProductExtensions
{
    public static string ToProductCategory(this string name)
    {
        if (name.ToLower().Contains("voice"))
        {
            return "Voice";
        }else if (name.ToLower().Contains("wifi"))
        {
            return "WiFi";
        }else if (name.ToLower().Contains("home"))
        {
            return "Home";
        }
        else
        {
            return "Other";
        }
    }
}
