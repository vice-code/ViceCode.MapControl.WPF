using System.Collections.Generic;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class CopyrightKeyComparer : IEqualityComparer<CopyrightKey>
    {
        public bool Equals(CopyrightKey first, CopyrightKey second) => first.Culture == second.Culture ? first.Style == second.Style : false;
        public int GetHashCode(CopyrightKey key) => (int)(key.Culture.GetHashCode() + key.Style);
    }
}
