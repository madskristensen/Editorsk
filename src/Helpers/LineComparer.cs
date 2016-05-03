using System;
using System.Collections.Generic;

namespace Editorsk
{
    public class LineComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (string.IsNullOrWhiteSpace(x) || string.IsNullOrWhiteSpace(y))
                return false;

            if (Object.ReferenceEquals(x, y)) return true;

            return string.Equals(x.Trim(), y.Trim(), StringComparison.CurrentCultureIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;

            return StringComparer.CurrentCultureIgnoreCase.GetHashCode(obj.Trim());
        }
    }
}