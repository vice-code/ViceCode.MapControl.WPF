using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Maps.MapExtras
{
    internal sealed class MemoryCache
    {
        private readonly object lockObj = new object();
        private readonly LinkedList<KeyValuePair<object, MemoryCacheValue>> age = new LinkedList<KeyValuePair<object, MemoryCacheValue>>();
        private readonly Dictionary<object, LinkedListNode<KeyValuePair<object, MemoryCacheValue>>> items = new Dictionary<object, LinkedListNode<KeyValuePair<object, MemoryCacheValue>>>();
        private int maxSize = 104857600;
        private int size;

        internal MemoryCache()
        {
        }

        public void Add(object key, MemoryCacheValue item)
        {
            lock (lockObj)
            {
                InternalAdd(key, item);
                Trim();
            }
        }

        public T GetValue<T>(object key) where T : MemoryCacheValue
        {
            lock (lockObj)
            {
                if (items.TryGetValue(key, out var node))
                {
                    age.Remove(node);
                    age.AddFirst(node);
                    return node.Value.Value as T;
                }
            }
            return default(T);
        }

        public void Remove(object key)
        {
            lock (lockObj)
                InternalRemove(key);
        }

        public void Replace(object key, MemoryCacheValue item)
        {
            lock (lockObj)
            {
                InternalRemove(key);
                InternalAdd(key, item);
                Trim();
            }
        }

        public static MemoryCache Instance { get; } = new MemoryCache();

        internal int MaxSize
        {
            get => maxSize;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                maxSize = value;
                Trim();
            }
        }

        private void Trim()
        {
            lock (lockObj)
            {
                while (size > maxSize)
                {
                    var last = age.Last;
                    age.RemoveLast();
                    items.Remove(last.Value.Key);
                    last.Value.Value.SizeChanged -= new EventHandler<MemoryCacheObjectSizeChangedEventArgs>(CacheEntrySizeChanged);
                    Interlocked.Add(ref size, -last.Value.Value.Size);
                }
            }
        }

        internal IEnumerable<MemoryCacheValue> ObjectsByAge
        {
            get
            {
                foreach (var keyValuePair in age)
                    yield return keyValuePair.Value;
            }
        }

        private void InternalAdd(object key, MemoryCacheValue item)
        {
            var linkedListNode = age.AddFirst(new KeyValuePair<object, MemoryCacheValue>(key, item));
            items.Add(key, linkedListNode);
            linkedListNode.Value.Value.SizeChanged += new EventHandler<MemoryCacheObjectSizeChangedEventArgs>(CacheEntrySizeChanged);
            Interlocked.Add(ref size, item.Size);
        }

        private void InternalRemove(object key)
        {
            if (!items.TryGetValue(key, out var node))
                return;
            items.Remove(key);
            age.Remove(node);
            node.Value.Value.SizeChanged -= new EventHandler<MemoryCacheObjectSizeChangedEventArgs>(CacheEntrySizeChanged);
            Interlocked.Add(ref size, -node.Value.Value.Size);
        }

        private void CacheEntrySizeChanged(object sender, MemoryCacheObjectSizeChangedEventArgs e)
        {
            if (Interlocked.Add(ref size, ((MemoryCacheValue)sender).Size - e.PreviousSize) <= maxSize)
                return;
            Trim();
        }
    }
}
