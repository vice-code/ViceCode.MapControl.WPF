using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maps.MapControl.WPF.Design;

namespace Microsoft.Maps.MapControl.WPF
{
    [TypeConverter(typeof(LocationCollectionConverter))]
    public class LocationCollection : ObservableCollection<Location>
    {
        public new void Add(Location location) => base.Add(location);
    }
}
