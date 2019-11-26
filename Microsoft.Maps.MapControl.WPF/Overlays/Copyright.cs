using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    public partial class Copyright : UserControl
    {
        private const char NonBreakingSpace = ' ';
        private ObservableCollection<AttributionInfo> attributions;

        public Copyright()
        {
            InitializeComponent();
            attributions = new ObservableCollection<AttributionInfo>();
            Attributions.CollectionChanged += new NotifyCollectionChangedEventHandler(Attributions_CollectionChanged);
        }

        internal ObservableCollection<AttributionInfo> Attributions
        {
            get => attributions;
            set
            {
                if (attributions is object)
                    attributions.CollectionChanged -= new NotifyCollectionChangedEventHandler(Attributions_CollectionChanged);
                attributions = value;
                if (attributions is object)
                    Attributions.CollectionChanged += new NotifyCollectionChangedEventHandler(Attributions_CollectionChanged);
                OnAttributionsChanged(null, null);
            }
        }

        private void Attributions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnAttributionsChanged(e.OldItems, e.NewItems);

        protected virtual void OnAttributionsChanged(IList oldItems, IList newItems)
        {
            AttributionsPanel.Items.Clear();
            foreach (var attribution in Attributions)
                AttributionsPanel.Items.Add(NonBreakingString(attribution.Text));
        }

        private static string NonBreakingString(string s)
        {
            var stringBuilder = new StringBuilder();
            for (var index = 0; index < s.Length; ++index)
            {
                var ch = s[index];
                if (' ' == ch)
                    ch = ' ';
                stringBuilder.Append(ch);
            }
            return stringBuilder.ToString();
        }
    }
}
