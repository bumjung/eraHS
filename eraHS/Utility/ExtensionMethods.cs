using System;
using System.Collections.Generic;

namespace eraHS.Utility
{
    public static class ExtensionMethods
    {
        public static void ShallowCopy<T>(this List<T> list, List<T> listToCopy)
        {
            foreach (T line in listToCopy)
            {
                list.Add(line);
            }
        }
    }
}
