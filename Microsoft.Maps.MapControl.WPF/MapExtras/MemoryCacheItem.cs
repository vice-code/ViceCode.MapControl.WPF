namespace Microsoft.Maps.MapExtras
{
    internal class MemoryCacheItem<T> : MemoryCacheValue
    {
        public MemoryCacheItem(T item, int size)
        {
            base.Size = size;
            Item = item;
        }

        public T Item { get; private set; }

        public new int Size
        {
            get => base.Size;
            set => base.Size = value;
        }
    }
}
