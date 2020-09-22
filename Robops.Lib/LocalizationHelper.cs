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

        public static DateTime ParseDatetime(string s)
        {
            if (DateTime.TryParse(s, ptBR, DateTimeStyles.None, out DateTime dt)) return dt;
            return DateTime.MinValue;
        }

    }
}
