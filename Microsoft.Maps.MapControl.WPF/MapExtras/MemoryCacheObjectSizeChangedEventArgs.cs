using System;

namespace Microsoft.Maps.MapExtras
{
    internal class MemoryCacheObjectSizeChangedEventArgs : EventArgs
    {
        public MemoryCacheObjectSizeChangedEventArgs(int previousSize) => PreviousSize = previousSize;

        public int PreviousSize { get; private set; }
    }
}
