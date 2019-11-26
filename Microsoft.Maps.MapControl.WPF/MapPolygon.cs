using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Maps.MapControl.WPF
{
    public class MapPolygon : MapShapeBase
    {
        public MapPolygon()
          : base(new Polygon())
        {
        }

        protected override PointCollection ProjectedPoints
        {
            get => ((Polygon)EncapsulatedShape).Points;
            set => ((Polygon)EncapsulatedShape).Points = value;
        }

        public FillRule FillRule
        {
            get => (FillRule)EncapsulatedShape.GetValue(Polygon.FillRuleProperty);
            set => EncapsulatedShape.SetValue(Polygon.FillRuleProperty, value);
        }
    }
}
