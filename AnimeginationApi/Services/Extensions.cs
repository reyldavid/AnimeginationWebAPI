﻿using System.Configuration;

namespace AnimeginationApi.Services
{
    public static class AnimeExtensions
    {
        //public static string GetConfigurationValue(this string appConfigKey)
        //{
        //    string sValue = ConfigurationManager.AppSettings[appConfigKey];

        //    if (sValue == null)
        //    {
        //        sValue = string.Empty;
        //    }
        //    return sValue;
        //}

        public static string GetConfigurationValue(this string appConfigKey)
        {
            string sValue = ConfigurationManager.AppSettings[appConfigKey] ?? string.Empty;

            return sValue;
        }

        public static int GetConfigurationNumericValue(this string appConfigKey)
        {
            int iNumber;
            string sNumber = ConfigurationManager.AppSettings[appConfigKey];

            int.TryParse(sNumber, out iNumber);
            return iNumber;
        }

        public static double GetConfigurationDoubleValue(this string appConfigKey)
        {
            double dAmount;
            string sAmount = ConfigurationManager.AppSettings[appConfigKey];

            double.TryParse(sAmount, out dAmount);

            return dAmount;
        }

        public static int GetNumericValue(this string sNumber)
        {
            int iNumber;
            int.TryParse(sNumber, out iNumber);
            return iNumber;
        }

        public static string SplitOnCase(this string mixedCases)
        {
            for (int i = 1; i < mixedCases.Length - 1; i++)
            {
                if ((char.IsLower(mixedCases[i - 1]) && char.IsUpper(mixedCases[i])) ||
                    (mixedCases[i - 1] != ' ' && char.IsUpper(mixedCases[i]) && char.IsLower(mixedCases[i + 1])))
                {
                    mixedCases = mixedCases.Insert(i, " ");
                }
            }
            return mixedCases;
        }

        public static string Singular(this string plural)
        {
            string singular = plural.Substring(plural.Length - 1, 1).ToLower().Equals("s")
                ? plural.Remove(plural.Length - 1, 1)
                : plural;
            return singular;
        }
    }
}
