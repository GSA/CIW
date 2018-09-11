using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Utilities
{
    public static class Extension
    {
        public static string removeItems(this string old, string[] toRemove)
        {
            string s = old;
            foreach (var c in toRemove)
            {
                s = s.Replace(c, string.Empty);
            }

            return s;
        }
    }
}
