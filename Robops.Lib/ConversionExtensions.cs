using Robops.Lib;
using System;

namespace Robos.Spiders.Extensions
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