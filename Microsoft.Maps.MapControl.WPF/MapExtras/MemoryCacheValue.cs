using System;

namespace Microsoft.Maps.MapExtras
{
    internal abstract class MemoryCacheValue
    {
        private int _size;

        public int Size
        {
            get => _size;
            protected set
            {
                if (_size == value)
                    return;
                var size = _size;
                _size = value;
                OnSizeChanged(size);
            }
        }

        internal event EventHandler<MemoryCacheObjectSizeChangedEventArgs> SizeChanged;

        protected void OnSizeChanged(int previousSize)
        {
            var sizeChanged = SizeChanged;
            if (sizeChanged is null)
                return;
            sizeChanged(this, new MemoryCacheObjectSizeChangedEventArgs(previousSize));
        }
    }
}
