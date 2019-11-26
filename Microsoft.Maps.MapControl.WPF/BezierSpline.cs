using System;
using System.Windows;

namespace Microsoft.Maps.MapControl.WPF
{
    internal class BezierSpline
    {
        private const double Accuracy = 0.001;
        private const double Fuzz = 1E-06;
        private double _parameter;
        private double _Bx;
        private double _Cx;
        private double _Cx_Bx;
        private double _three_Cx;
        private double _By;
        private double _Cy;

        public Point ControlPoint1 { get; private set; }

        public Point ControlPoint2 { get; private set; }

        public BezierSpline(Point controlPoint1, Point controlPoint2)
        {
            ControlPoint1 = controlPoint1;
            ControlPoint2 = controlPoint2;
            Build();
        }

        public double GetValue(double progress)
        {
            SetParameterFromX(progress);
            return GetBezierValue(_By, _Cy, _parameter);
        }

        private static double GetBezierValue(double b, double c, double t)
        {
            var num1 = 1.0 - t;
            var num2 = t * t;
            return b * t * num1 * num1 + c * num2 * num1 + num2 * t;
        }

        private void GetXAndDx(double t, out double x, out double dx)
        {
            var num1 = 1.0 - t;
            var num2 = t * t;
            var num3 = num1 * num1;
            x = _Bx * t * num3 + _Cx * num2 * num1 + num2 * t;
            dx = _Bx * num3 + _Cx_Bx * num1 * t + _three_Cx * num2;
        }

        private void SetParameterFromX(double time)
        {
            var num1 = 0.0;
            var num2 = 1.0;
            if (time == 0.0)
                _parameter = 0.0;
            else if (time == 1.0)
            {
                _parameter = 1.0;
            }
            else
            {
                while (num2 - num1 > 1E-06)
                {
                    GetXAndDx(_parameter, out var x, out var dx);
                    var num3 = Math.Abs(dx);
                    if (x > time)
                        num2 = _parameter;
                    else
                        num1 = _parameter;
                    if (Math.Abs(x - time) < 0.001 * num3)
                        break;
                    if (num3 > 1E-06)
                    {
                        var num4 = _parameter - (x - time) / dx;
                        _parameter = num4 < num2 ? (num4 > num1 ? num4 : (_parameter + num1) / 2.0) : (_parameter + num2) / 2.0;
                    }
                    else
                        _parameter = (num1 + num2) / 2.0;
                }
            }
        }

        private void Build()
        {
            _parameter = 0.0;
            _Bx = 3.0 * ControlPoint1.X;
            _Cx = 3.0 * ControlPoint2.X;
            _Cx_Bx = 2.0 * (_Cx - _Bx);
            _three_Cx = 3.0 - _Cx;
            _By = 3.0 * ControlPoint1.Y;
            _Cy = 3.0 * ControlPoint2.Y;
        }
    }
}
