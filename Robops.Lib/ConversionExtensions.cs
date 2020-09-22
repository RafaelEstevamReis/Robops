using System;

namespace Robops.Lib
{
    public static class ConversionExtensions
    {
        public static int ToInt(this string Text)
        {
            return int.Parse(Text);
        }
        public static decimal ToDecimal(this string s)
        {
            return LocalizationHelper.ParseDecimal(s);
        }
        public static DateTime ToDateTime(this string s)
        {
            return LocalizationHelper.ParseDatetime(s);
        }
    }
}
