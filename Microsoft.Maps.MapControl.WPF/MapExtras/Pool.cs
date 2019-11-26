using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Maps.MapExtras
{
    internal class Pool<TKey, TValue>
    {
        private readonly Dictionary<TKey, LinkedList<PooledItem>> pooledItems = new Dictionary<TKey, LinkedList<PooledItem>>();
        private const int SecondsBeforeRemoval = 10;
        private const int MaxItemCount = 100;
        private readonly DispatcherTimer garbageCollectionTimer;

        public Pool()
        {
            garbageCollectionTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            garbageCollectionTimer.Tick += new EventHandler(GarbageCollectionTimer_Tick);
            garbageCollectionTimer.Start();
        }

        public void Add(TKey key, TValue value)
        {
            if (!pooledItems.TryGetValue(key, out var linkedList))
            {
                linkedList = new LinkedList<PooledItem>();
                pooledItems[key] = linkedList;
            }
            linkedList.AddLast(new PooledItem()
            {
                TimeWhenAdded = DateTime.UtcNow,
                Item = value
            });
            if (linkedList.Count <= 100)
                return;
            linkedList.RemoveFirst();
        }

        public TValue Get(TKey key)
        {
            var pooledItem = new PooledItem();
            if (pooledItems.TryGetValue(key, out var linkedList) && linkedList.Count > 0)
            {
                pooledItem = linkedList.First.Value;
                linkedList.RemoveFirst();
            }
            return pooledItem.Item;
        }

        private void GarbageCollectionTimer_Tick(object sender, EventArgs e)
        {
            var utcNow = DateTime.UtcNow;
            LinkedListNode<PooledItem> next;
            foreach (var linkedList in pooledItems.Values)
            {
                for (var node = linkedList.First; node is object; node = next)
                {
                    next = node.Next;
                    if ((utcNow - node.Value.TimeWhenAdded).TotalSeconds > 10.0)
                        linkedList.Remove(node);
                }
            }
        }

        private struct PooledItem
        {
            public TValue Item;
            public DateTime TimeWhenAdded;
        }
    }
}
