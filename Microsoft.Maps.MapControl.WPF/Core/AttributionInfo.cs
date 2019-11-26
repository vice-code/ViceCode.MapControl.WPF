namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class AttributionInfo
    {
        public AttributionInfo() { }

        public AttributionInfo(string text) => Text = text;

        public string Text { get; set; }

        public static bool operator ==(AttributionInfo attributionInfo1, AttributionInfo attributionInfo2)
        {
            if (ReferenceEquals(attributionInfo1, attributionInfo2))
                return true;
            if (attributionInfo1 is null || attributionInfo2 is null)
                return false;
            return attributionInfo1.Text == attributionInfo2.Text;
        }

        public static bool operator !=(AttributionInfo attributionInfo1, AttributionInfo attributionInfo2) => !(attributionInfo1 == attributionInfo2);

        public override bool Equals(object obj)
        {
            if (!(obj is AttributionInfo attributionInfo))
                return false;
            return this == attributionInfo;
        }

        public override int GetHashCode() => (Text ?? string.Empty).GetHashCode(System.StringComparison.CurrentCulture);
    }
}
