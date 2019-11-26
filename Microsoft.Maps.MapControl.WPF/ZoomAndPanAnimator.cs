using System;
using System.Windows;

namespace Microsoft.Maps.MapControl.WPF
{
    internal sealed class ZoomAndPanAnimator
    {
        private readonly BezierSpline VelocitySpline;
        private Point c0;
        private Point c1;
        private Point direction;
        private double w0;
        private double w1;
        private double u0;
        private double u1;
        private double b0;
        private double b1;
        private double r0;
        private double r1;
        private double S;

        public double Rho { get; set; }

        public double AverageVelocity { get; set; }

        public ZoomAndPanAnimator()
        {
            Rho = Math.Sqrt(1.9);
            AverageVelocity = 2.0;
            VelocitySpline = new BezierSpline(new Point(0.45, 0.35), new Point(0.0, 1.0));
        }

        public void Begin(Rect fromRect, Rect toRect, out double duration)
        {
            c0 = new Point(fromRect.X + 0.5 * fromRect.Width, fromRect.Y + 0.5 * fromRect.Height);
            c1 = new Point(toRect.X + 0.5 * toRect.Width, toRect.Y + 0.5 * toRect.Height);
            w0 = fromRect.Width;
            w1 = toRect.Width;
            direction.X = c1.X - c0.X;
            direction.Y = c1.Y - c0.Y;
            if (Math.Max(Math.Abs(direction.X), Math.Abs(direction.Y)) < 1E-06)
            {
                u0 = 0.0;
                u1 = 0.0;
                S = Math.Abs(Math.Log(w1 / w0)) / Rho;
            }
            else
            {
                u0 = 0.0;
                u1 = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                b0 = (w1 * w1 - w0 * w0 + Rho * Rho * Rho * Rho * u1 * u1) / (2.0 * w0 * Rho * Rho * u1);
                b1 = (w1 * w1 - w0 * w0 - Rho * Rho * Rho * Rho * u1 * u1) / (2.0 * w1 * Rho * Rho * u1);
                r0 = Math.Log(-b0 + Math.Sqrt(b0 * b0 + 1.0));
                r1 = Math.Log(-b1 + Math.Sqrt(b1 * b1 + 1.0));
                S = (r1 - r0) / Rho;
            }
            duration = S / AverageVelocity;
            if (!double.IsNaN(S) && duration <= 20.0)
                return;
            duration = 20.0;
            S = duration * AverageVelocity;
        }

        public void Tick(double fractionComplete, out double width, out Point center)
        {
            var num1 = VelocitySpline.GetValue(fractionComplete) * S;
            double num2;
            if (this.u0 == u1)
            {
                var u0 = this.u0;
                num2 = w0 * Math.Exp((w1 < w0 ? -1.0 : 1.0) * Rho * num1);
                center = c0;
            }
            else
            {
                var num3 = w0 / (Rho * Rho) * Math.Cosh(r0) * Math.Tanh(Rho * num1 + r0) - w0 / (Rho * Rho) * Math.Sinh(r0) + u0;
                num2 = w0 * Math.Cosh(r0) / Math.Cosh(Rho * num1 + r0);
                center = new Point(c0.X + direction.X * num3 / u1, c0.Y + direction.Y * num3 / u1);
            }
            width = num2;
        }
    }
}
