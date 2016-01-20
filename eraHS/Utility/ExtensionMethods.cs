using System;
using System.Collections.Generic;

namespace eraHS.Utility
{
    static class ExtensionMethods
    {
        public static void ShallowCopy<T>(this List<T> list, List<T> listToCopy)
        {
            foreach (T line in listToCopy)
            {
                list.Add(line);
            }
        }

        public static string Simplify(this String str)
        {
            return str.Replace(' ', '_').ToLower();
        }

        public static DateTime ConvertStringToDateTime(this string str)
        {
            DateTime today = DateTime.Today;
            string stringTime = str;
            stringTime = stringTime.Substring(0, stringTime.IndexOf('.'));
            String[] time = stringTime.Split(':');
            return new DateTime(today.Year, today.Month, today.Day,
                Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]));
        }
    }
}
