using System;
using System.Globalization;

namespace Robops.Lib
{
    public static class LocalizationHelper
    {
        public static readonly CultureInfo ptBR = new CultureInfo("pt-BR");

        public static decimal ParseDecimal(string s)
        {
            if (s.Contains("R$")) s = s.Split('$')[1];
            return decimal.Parse(s, ptBR);
        }
        public static decimal ParseDecimal(string s, decimal Error)
        {
            if (s.Contains("R$")) s = s.Split('$')[1];
            if (decimal.TryParse(s, NumberStyles.Any, ptBR, out decimal val))
                return val;
            return Error;
        }

        public static DateTime ParseDatetime(string s)
        {
            if (DateTime.TryParse(s, ptBR, DateTimeStyles.None, out DateTime dt)) return dt;
            return DateTime.MinValue;
        }
    }
}
