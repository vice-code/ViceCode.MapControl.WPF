using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Maps.MapControl.WPF
{
    public class MapPolyline : MapShapeBase
    {
        public MapPolyline()
          : base(new Polyline())
        {
        }

        protected override PointCollection ProjectedPoints
        {
            get => ((Polyline)EncapsulatedShape).Points;
            set => ((Polyline)EncapsulatedShape).Points = value;
        }

        public FillRule FillRule
        {
            get => (FillRule)EncapsulatedShape.GetValue(Polyline.FillRuleProperty);
            set => EncapsulatedShape.SetValue(Polyline.FillRuleProperty, value);
        }
    }
}
