using System.Text.RegularExpressions;

namespace RNPM.API.Utilities.Extensions;

public static class ValidationExtensions
{
    public static bool IsEconet(this string val)
    {
        return Regex.IsMatch(val, @"^((00|\+)?(263))?0?7(7|8)[0-9]{7}$");
    }

    public static bool IsNetone(this string val)
    {
        return Regex.IsMatch(val, @"^((00|\+)?(263))?0?71[0-9]{7}$");
    }

    public static bool IsTelecel(this string val)
    {
        return Regex.IsMatch(val, @"^((00|\+)?(263))?0?73[0-9]{7}$");
    }

    public static string GetMobilePaymentMethod(this string val)
    {
        var phone = val.Trim().Replace(" ", string.Empty);

        return true switch
        {
            bool _ when Regex.IsMatch(phone, @"^((00|\+)?(263))?0?7(7|8)[0-9]{7}$") => "ecocash",
            bool _ when Regex.IsMatch(phone, @"^((00|\+)?(263))?0?71[0-9]{7}$") => "onemoney",
            bool _ when Regex.IsMatch(phone, @"^((00|\+)?(263))?0?73[0-9]{7}$") => "telecash",
            _ => string.Empty,
        };
    }

    public static string GetMsisdn(this string val, string prefix)
    {
        var phone = val.Trim().Replace(" ", string.Empty);

        var result = phone.Substring(phone.Length - 9);

        return $"{prefix}{result}";
    }

    public static bool IsPhone(this string val)
    {
        if (string.IsNullOrEmpty(val)) return false;

        var number  = val.Trim().Replace(" ", string.Empty);

        return Regex.IsMatch(number, @"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$");
    }

    public static bool IsEmail(this string val)
    {
        return !string.IsNullOrEmpty(val) && Regex.IsMatch(val, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
    }
}