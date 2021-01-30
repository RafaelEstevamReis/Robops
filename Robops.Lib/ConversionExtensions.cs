using System;

namespace Robops.Lib
{
    public static class ConversionExtensions
    {
        public static int ToInt(this string Text)
        {
            return int.Parse(Text);
        }
        public static int ToInt(this string Text, int Error)
        {
            if (int.TryParse(Text, out int val)) return val;
            return 0;
        }
        public static decimal ToDecimal(this string s)
        {
            return LocalizationHelper.ParseDecimal(s);
        }
        public static decimal ToDecimal(this string s, decimal Error)
        {
            return LocalizationHelper.ParseDecimal(s, Error);
        }
        public static DateTime ToDateTime(this string s)
        {
            return LocalizationHelper.ParseDatetime(s);
        }
    }
}
