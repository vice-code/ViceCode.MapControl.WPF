using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class AttributionCollection : ReadOnlyObservableCollection<AttributionInfo>
    {
        public AttributionCollection(ObservableCollection<AttributionInfo> list) : base(list) { }

        public new event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnCollectionChanged(args);
            if (CollectionChanged is null)
                return;
            CollectionChanged(this, args);
        }
    }
}
